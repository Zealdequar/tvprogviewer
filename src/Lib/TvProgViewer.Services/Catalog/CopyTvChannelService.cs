using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Copy TvChannel service
    /// </summary>
    public partial class CopyTvChannelService : ICopyTvChannelService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICategoryService _categoryService;
        private readonly IDownloadService _downloadService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;
        private readonly ITvChannelTagService _tvChannelTagService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVideoService _videoService;

        #endregion

        #region Ctor

        public CopyTvChannelService(IAclService aclService,
            ICategoryService categoryService,
            IDownloadService downloadService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService,
            ITvChannelTagService tvChannelTagService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IVideoService videoService)
        {
            _aclService = aclService;
            _categoryService = categoryService;
            _downloadService = downloadService;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
            _tvChannelTagService = tvChannelTagService;
            _specificationAttributeService = specificationAttributeService;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _videoService = videoService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Copy discount mappings
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyDiscountsMappingAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            foreach (var discountMapping in await _tvChannelService.GetAllDiscountsAppliedToTvChannelAsync(tvChannel.Id))
            {
                await _tvChannelService.InsertDiscountTvChannelMappingAsync(new DiscountTvChannelMapping { EntityId = tvChannelCopy.Id, DiscountId = discountMapping.DiscountId });
                await _tvChannelService.UpdateTvChannelAsync(tvChannelCopy);
            }
        }

        /// <summary>
        /// Copy associated tvChannels
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="isPublished">A value indicating whether they should be published</param>
        /// <param name="copyMultimedia">A value indicating whether to copy images and videos</param>
        /// <param name="copyAssociatedTvChannels">A value indicating whether to copy associated tvChannels</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyAssociatedTvChannelsAsync(TvChannel tvChannel, bool isPublished, bool copyMultimedia, bool copyAssociatedTvChannels, TvChannel tvChannelCopy)
        {
            if (!copyAssociatedTvChannels)
                return;

            var associatedTvChannels = await _tvChannelService.GetAssociatedTvChannelsAsync(tvChannel.Id, showHidden: true);
            foreach (var associatedTvChannel in associatedTvChannels)
            {
                var associatedTvChannelCopy = await CopyTvChannelAsync(associatedTvChannel,
                    string.Format(TvProgCatalogDefaults.TvChannelCopyNameTemplate, associatedTvChannel.Name),
                    isPublished, copyMultimedia, false);
                associatedTvChannelCopy.ParentGroupedTvChannelId = tvChannelCopy.Id;
                await _tvChannelService.UpdateTvChannelAsync(associatedTvChannelCopy);
            }
        }

        /// <summary>
        /// Copy tier prices
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyTierPricesAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            foreach (var tierPrice in await _tvChannelService.GetTierPricesByTvChannelAsync(tvChannel.Id))
                await _tvChannelService.InsertTierPriceAsync(new TierPrice
                {
                    TvChannelId = tvChannelCopy.Id,
                    StoreId = tierPrice.StoreId,
                    UserRoleId = tierPrice.UserRoleId,
                    Quantity = tierPrice.Quantity,
                    Price = tierPrice.Price,
                    StartDateTimeUtc = tierPrice.StartDateTimeUtc,
                    EndDateTimeUtc = tierPrice.EndDateTimeUtc
                });
        }

        /// <summary>
        /// Copy attributes mapping
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <param name="originalNewPictureIdentifiers">Identifiers of pictures</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyAttributesMappingAsync(TvChannel tvChannel, TvChannel tvChannelCopy, Dictionary<int, int> originalNewPictureIdentifiers)
        {
            var associatedAttributes = new Dictionary<int, int>();
            var associatedAttributeValues = new Dictionary<int, int>();

            //attribute mapping with condition attributes
            var oldCopyWithConditionAttributes = new List<TvChannelAttributeMapping>();

            //all tvChannel attribute mapping copies
            var tvChannelAttributeMappingCopies = new Dictionary<int, TvChannelAttributeMapping>();

            var languages = await _languageService.GetAllLanguagesAsync(true);

            foreach (var tvChannelAttributeMapping in await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id))
            {
                var tvChannelAttributeMappingCopy = new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelCopy.Id,
                    TvChannelAttributeId = tvChannelAttributeMapping.TvChannelAttributeId,
                    TextPrompt = tvChannelAttributeMapping.TextPrompt,
                    IsRequired = tvChannelAttributeMapping.IsRequired,
                    AttributeControlTypeId = tvChannelAttributeMapping.AttributeControlTypeId,
                    DisplayOrder = tvChannelAttributeMapping.DisplayOrder,
                    ValidationMinLength = tvChannelAttributeMapping.ValidationMinLength,
                    ValidationMaxLength = tvChannelAttributeMapping.ValidationMaxLength,
                    ValidationFileAllowedExtensions = tvChannelAttributeMapping.ValidationFileAllowedExtensions,
                    ValidationFileMaximumSize = tvChannelAttributeMapping.ValidationFileMaximumSize,
                    DefaultValue = tvChannelAttributeMapping.DefaultValue
                };
                await _tvChannelAttributeService.InsertTvChannelAttributeMappingAsync(tvChannelAttributeMappingCopy);
                //localization
                foreach (var lang in languages)
                {
                    var textPrompt = await _localizationService.GetLocalizedAsync(tvChannelAttributeMapping, x => x.TextPrompt, lang.Id, false, false);
                    if (!string.IsNullOrEmpty(textPrompt))
                        await _localizedEntityService.SaveLocalizedValueAsync(tvChannelAttributeMappingCopy, x => x.TextPrompt, textPrompt,
                            lang.Id);
                }

                tvChannelAttributeMappingCopies.Add(tvChannelAttributeMappingCopy.Id, tvChannelAttributeMappingCopy);

                if (!string.IsNullOrEmpty(tvChannelAttributeMapping.ConditionAttributeXml))
                {
                    oldCopyWithConditionAttributes.Add(tvChannelAttributeMapping);
                }

                //save associated value (used for combinations copying)
                associatedAttributes.Add(tvChannelAttributeMapping.Id, tvChannelAttributeMappingCopy.Id);

                // tvChannel attribute values
                var tvChannelAttributeValues = await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(tvChannelAttributeMapping.Id);
                foreach (var tvChannelAttributeValue in tvChannelAttributeValues)
                {
                    var attributeValuePictureId = 0;
                    if (originalNewPictureIdentifiers.ContainsKey(tvChannelAttributeValue.PictureId)) 
                        attributeValuePictureId = originalNewPictureIdentifiers[tvChannelAttributeValue.PictureId];

                    var attributeValueCopy = new TvChannelAttributeValue
                    {
                        TvChannelAttributeMappingId = tvChannelAttributeMappingCopy.Id,
                        AttributeValueTypeId = tvChannelAttributeValue.AttributeValueTypeId,
                        AssociatedTvChannelId = tvChannelAttributeValue.AssociatedTvChannelId,
                        Name = tvChannelAttributeValue.Name,
                        ColorSquaresRgb = tvChannelAttributeValue.ColorSquaresRgb,
                        PriceAdjustment = tvChannelAttributeValue.PriceAdjustment,
                        PriceAdjustmentUsePercentage = tvChannelAttributeValue.PriceAdjustmentUsePercentage,
                        WeightAdjustment = tvChannelAttributeValue.WeightAdjustment,
                        Cost = tvChannelAttributeValue.Cost,
                        UserEntersQty = tvChannelAttributeValue.UserEntersQty,
                        Quantity = tvChannelAttributeValue.Quantity,
                        IsPreSelected = tvChannelAttributeValue.IsPreSelected,
                        DisplayOrder = tvChannelAttributeValue.DisplayOrder,
                        PictureId = attributeValuePictureId,
                    };
                    //picture associated to "iamge square" attribute type (if exists)
                    if (tvChannelAttributeValue.ImageSquaresPictureId > 0)
                    {
                        var origImageSquaresPicture =
                            await _pictureService.GetPictureByIdAsync(tvChannelAttributeValue.ImageSquaresPictureId);
                        if (origImageSquaresPicture != null)
                        {
                            //copy the picture
                            var imageSquaresPictureCopy = await _pictureService.InsertPictureAsync(
                                await _pictureService.LoadPictureBinaryAsync(origImageSquaresPicture),
                                origImageSquaresPicture.MimeType,
                                origImageSquaresPicture.SeoFilename,
                                origImageSquaresPicture.AltAttribute,
                                origImageSquaresPicture.TitleAttribute);
                            attributeValueCopy.ImageSquaresPictureId = imageSquaresPictureCopy.Id;
                        }
                    }

                    await _tvChannelAttributeService.InsertTvChannelAttributeValueAsync(attributeValueCopy);

                    //save associated value (used for combinations copying)
                    associatedAttributeValues.Add(tvChannelAttributeValue.Id, attributeValueCopy.Id);

                    //localization
                    foreach (var lang in languages)
                    {
                        var name = await _localizationService.GetLocalizedAsync(tvChannelAttributeValue, x => x.Name, lang.Id, false, false);
                        if (!string.IsNullOrEmpty(name))
                            await _localizedEntityService.SaveLocalizedValueAsync(attributeValueCopy, x => x.Name, name, lang.Id);
                    }
                }
            }

            //copy attribute conditions
            foreach (var tvChannelAttributeMapping in oldCopyWithConditionAttributes)
            {
                var oldConditionAttributeMapping = (await _tvChannelAttributeParser
                    .ParseTvChannelAttributeMappingsAsync(tvChannelAttributeMapping.ConditionAttributeXml)).FirstOrDefault();

                if (oldConditionAttributeMapping == null)
                    continue;

                var oldConditionValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(
                    tvChannelAttributeMapping.ConditionAttributeXml,
                    oldConditionAttributeMapping.Id);

                if (!oldConditionValues.Any())
                    continue;

                var newAttributeMappingId = associatedAttributes[oldConditionAttributeMapping.Id];
                var newConditionAttributeMapping = tvChannelAttributeMappingCopies[newAttributeMappingId];

                var newConditionAttributeXml = string.Empty;

                foreach (var oldConditionValue in oldConditionValues)
                {
                    newConditionAttributeXml = _tvChannelAttributeParser.AddTvChannelAttribute(newConditionAttributeXml,
                        newConditionAttributeMapping, associatedAttributeValues[oldConditionValue.Id].ToString());
                }

                var attributeMappingId = associatedAttributes[tvChannelAttributeMapping.Id];
                var conditionAttribute = tvChannelAttributeMappingCopies[attributeMappingId];
                conditionAttribute.ConditionAttributeXml = newConditionAttributeXml;

                await _tvChannelAttributeService.UpdateTvChannelAttributeMappingAsync(conditionAttribute);
            }

            //attribute combinations
            foreach (var combination in await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id))
            {
                //generate new AttributesXml according to new value IDs
                var newAttributesXml = string.Empty;
                var parsedTvChannelAttributes = await _tvChannelAttributeParser.ParseTvChannelAttributeMappingsAsync(combination.AttributesXml);
                foreach (var oldAttribute in parsedTvChannelAttributes)
                {
                    if (!associatedAttributes.ContainsKey(oldAttribute.Id))
                        continue;

                    var newAttribute = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(associatedAttributes[oldAttribute.Id]);

                    if (newAttribute == null)
                        continue;

                    var oldAttributeValuesStr = _tvChannelAttributeParser.ParseValues(combination.AttributesXml, oldAttribute.Id);

                    foreach (var oldAttributeValueStr in oldAttributeValuesStr)
                    {
                        if (newAttribute.ShouldHaveValues())
                        {
                            //attribute values
                            var oldAttributeValue = int.Parse(oldAttributeValueStr);
                            if (!associatedAttributeValues.ContainsKey(oldAttributeValue))
                                continue;

                            var newAttributeValue = await _tvChannelAttributeService.GetTvChannelAttributeValueByIdAsync(associatedAttributeValues[oldAttributeValue]);

                            if (newAttributeValue != null)
                            {
                                newAttributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(newAttributesXml,
                                    newAttribute, newAttributeValue.Id.ToString());
                            }
                        }
                        else
                        {
                            //just a text
                            newAttributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(newAttributesXml,
                                newAttribute, oldAttributeValueStr);
                        }
                    }
                }

                //picture
                originalNewPictureIdentifiers.TryGetValue(combination.PictureId, out var combinationPictureId);

                var combinationCopy = new TvChannelAttributeCombination
                {
                    TvChannelId = tvChannelCopy.Id,
                    AttributesXml = newAttributesXml,
                    StockQuantity = combination.StockQuantity,
                    MinStockQuantity = combination.MinStockQuantity,
                    AllowOutOfStockOrders = combination.AllowOutOfStockOrders,
                    Sku = combination.Sku,
                    ManufacturerPartNumber = combination.ManufacturerPartNumber,
                    Gtin = combination.Gtin,
                    OverriddenPrice = combination.OverriddenPrice,
                    NotifyAdminForQuantityBelow = combination.NotifyAdminForQuantityBelow,
                    PictureId = combinationPictureId
                };
                await _tvChannelAttributeService.InsertTvChannelAttributeCombinationAsync(combinationCopy);

                //quantity change history
                await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannelCopy, combination.StockQuantity,
                    combination.StockQuantity,
                    message: string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyTvChannel"), tvChannel.Id), combinationId: combination.Id);
            }
        }

        /// <summary>
        /// Copy tvChannel specifications
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyTvChannelSpecificationsAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync();

            foreach (var tvChannelSpecificationAttribute in await _specificationAttributeService.GetTvChannelSpecificationAttributesAsync(tvChannel.Id))
            {
                var psaCopy = new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelCopy.Id,
                    AttributeTypeId = tvChannelSpecificationAttribute.AttributeTypeId,
                    SpecificationAttributeOptionId = tvChannelSpecificationAttribute.SpecificationAttributeOptionId,
                    CustomValue = tvChannelSpecificationAttribute.CustomValue,
                    AllowFiltering = tvChannelSpecificationAttribute.AllowFiltering,
                    ShowOnTvChannelPage = tvChannelSpecificationAttribute.ShowOnTvChannelPage,
                    DisplayOrder = tvChannelSpecificationAttribute.DisplayOrder
                };

                await _specificationAttributeService.InsertTvChannelSpecificationAttributeAsync(psaCopy);
                
                foreach (var language in allLanguages)
                {
                    var customValue = await _localizationService.GetLocalizedAsync(tvChannelSpecificationAttribute, x => x.CustomValue, language.Id, false, false);
                    if (!string.IsNullOrEmpty(customValue))
                        await _localizedEntityService.SaveLocalizedValueAsync(psaCopy, x => x.CustomValue, customValue, language.Id);
                }
            }
        }

        /// <summary>
        /// Copy crosssell mapping
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyCrossSellsMappingAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            foreach (var csTvChannel in await _tvChannelService.GetCrossSellTvChannelsByTvChannelId1Async(tvChannel.Id, true))
                await _tvChannelService.InsertCrossSellTvChannelAsync(
                    new CrossSellTvChannel
                    {
                        TvChannelId1 = tvChannelCopy.Id,
                        TvChannelId2 = csTvChannel.TvChannelId2
                    });
        }

        /// <summary>
        /// Copy related tvChannels mapping
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyRelatedTvChannelsMappingAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            foreach (var relatedTvChannel in await _tvChannelService.GetRelatedTvChannelsByTvChannelId1Async(tvChannel.Id, true))
                await _tvChannelService.InsertRelatedTvChannelAsync(
                    new RelatedTvChannel
                    {
                        TvChannelId1 = tvChannelCopy.Id,
                        TvChannelId2 = relatedTvChannel.TvChannelId2,
                        DisplayOrder = relatedTvChannel.DisplayOrder
                    });
        }

        /// <summary>
        /// Copy manufacturer mapping
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyManufacturersMappingAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            foreach (var tvChannelManufacturers in await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvChannel.Id, true))
            {
                var tvChannelManufacturerCopy = new TvChannelManufacturer
                {
                    TvChannelId = tvChannelCopy.Id,
                    ManufacturerId = tvChannelManufacturers.ManufacturerId,
                    IsFeaturedTvChannel = tvChannelManufacturers.IsFeaturedTvChannel,
                    DisplayOrder = tvChannelManufacturers.DisplayOrder
                };

                await _manufacturerService.InsertTvChannelManufacturerAsync(tvChannelManufacturerCopy);
            }
        }

        /// <summary>
        /// Copy category mapping
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyCategoriesMappingAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            foreach (var tvChannelCategory in await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvChannel.Id, showHidden: true))
            {
                var tvChannelCategoryCopy = new TvChannelCategory
                {
                    TvChannelId = tvChannelCopy.Id,
                    CategoryId = tvChannelCategory.CategoryId,
                    IsFeaturedTvChannel = tvChannelCategory.IsFeaturedTvChannel,
                    DisplayOrder = tvChannelCategory.DisplayOrder
                };

                await _categoryService.InsertTvChannelCategoryAsync(tvChannelCategoryCopy);
            }
        }

        /// <summary>
        /// Copy warehouse mapping
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyWarehousesMappingAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            foreach (var pwi in await _tvChannelService.GetAllTvChannelWarehouseInventoryRecordsAsync(tvChannel.Id))
            {
                await _tvChannelService.InsertTvChannelWarehouseInventoryAsync(
                    new TvChannelWarehouseInventory
                    {
                        TvChannelId = tvChannelCopy.Id,
                        WarehouseId = pwi.WarehouseId,
                        StockQuantity = pwi.StockQuantity,
                        ReservedQuantity = 0
                    });

                //quantity change history
                var message = $"{await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyTvChannel"), tvChannel.Id)}";
                await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannelCopy, pwi.StockQuantity, pwi.StockQuantity, pwi.WarehouseId, message);
            }

            await _tvChannelService.UpdateTvChannelAsync(tvChannelCopy);
        }

        /// <summary>
        /// Copy tvChannel pictures
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="newName">New tvChannel name</param>
        /// <param name="copyMultimedia"></param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the identifiers of old and new pictures
        /// </returns>
        protected virtual async Task<Dictionary<int, int>> CopyTvChannelPicturesAsync(TvChannel tvChannel, string newName, bool copyMultimedia, TvChannel tvChannelCopy)
        {
            //variable to store original and new picture identifiers
            var originalNewPictureIdentifiers = new Dictionary<int, int>();
            if (!copyMultimedia)
                return originalNewPictureIdentifiers;

            foreach (var tvChannelPicture in await _tvChannelService.GetTvChannelPicturesByTvChannelIdAsync(tvChannel.Id))
            {
                var picture = await _pictureService.GetPictureByIdAsync(tvChannelPicture.PictureId);
                var pictureCopy = await _pictureService.InsertPictureAsync(
                    await _pictureService.LoadPictureBinaryAsync(picture),
                    picture.MimeType,
                    await _pictureService.GetPictureSeNameAsync(newName),
                    picture.AltAttribute,
                    picture.TitleAttribute);
                await _tvChannelService.InsertTvChannelPictureAsync(new TvChannelPicture
                {
                    TvChannelId = tvChannelCopy.Id,
                    PictureId = pictureCopy.Id,
                    DisplayOrder = tvChannelPicture.DisplayOrder
                });
                originalNewPictureIdentifiers.Add(picture.Id, pictureCopy.Id);
            }

            return originalNewPictureIdentifiers;
        }

        /// <summary>
        /// Copy tvChannel videos
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="copyVideos"></param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyTvChannelVideosAsync(TvChannel tvChannel, bool copyVideos, TvChannel tvChannelCopy)
        {
            if (copyVideos)
            {
                foreach (var tvChannelVideo in await _tvChannelService.GetTvChannelVideosByTvChannelIdAsync(tvChannel.Id))
                {
                    var video = await _videoService.GetVideoByIdAsync(tvChannelVideo.VideoId);
                    var videoCopy = await _videoService.InsertVideoAsync(video);
                    await _tvChannelService.InsertTvChannelVideoAsync(new TvChannelVideo
                    {
                        TvChannelId = tvChannelCopy.Id,
                        VideoId = videoCopy.Id,
                        DisplayOrder = tvChannelVideo.DisplayOrder
                    });
                }
            }
        }

        /// <summary>
        /// Copy localization data
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelCopy">New tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyLocalizationDataAsync(TvChannel tvChannel, TvChannel tvChannelCopy)
        {
            var languages = await _languageService.GetAllLanguagesAsync(true);

            //localization
            foreach (var lang in languages)
            {
                var name = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name, lang.Id, false, false);
                if (!string.IsNullOrEmpty(name))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvChannelCopy, x => x.Name, name, lang.Id);

                var shortDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.ShortDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(shortDescription))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvChannelCopy, x => x.ShortDescription, shortDescription, lang.Id);

                var fullDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.FullDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(fullDescription))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvChannelCopy, x => x.FullDescription, fullDescription, lang.Id);

                var metaKeywords = await _localizationService.GetLocalizedAsync(tvChannel, x => x.MetaKeywords, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaKeywords))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvChannelCopy, x => x.MetaKeywords, metaKeywords, lang.Id);

                var metaDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.MetaDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaDescription))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvChannelCopy, x => x.MetaDescription, metaDescription, lang.Id);

                var metaTitle = await _localizationService.GetLocalizedAsync(tvChannel, x => x.MetaTitle, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaTitle))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvChannelCopy, x => x.MetaTitle, metaTitle, lang.Id);

                //search engine name
                await _urlRecordService.SaveSlugAsync(tvChannelCopy, await _urlRecordService.ValidateSeNameAsync(tvChannelCopy, string.Empty, name, false), lang.Id);
            }
        }

        /// <summary>
        /// Copy tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="newName">New tvChannel name</param>
        /// <param name="isPublished">A value indicating whether a new tvChannel is published</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        protected virtual async Task<TvChannel> CopyBaseTvChannelDataAsync(TvChannel tvChannel, string newName, bool isPublished)
        {
            //tvChannel download & sample download
            var downloadId = tvChannel.DownloadId;
            var sampleDownloadId = tvChannel.SampleDownloadId;
            if (tvChannel.IsDownload)
            {
                var download = await _downloadService.GetDownloadByIdAsync(tvChannel.DownloadId);
                if (download != null)
                {
                    var downloadCopy = new Download
                    {
                        DownloadGuid = Guid.NewGuid(),
                        UseDownloadUrl = download.UseDownloadUrl,
                        DownloadUrl = download.DownloadUrl,
                        DownloadBinary = download.DownloadBinary,
                        ContentType = download.ContentType,
                        Filename = download.Filename,
                        Extension = download.Extension,
                        IsNew = download.IsNew
                    };
                    await _downloadService.InsertDownloadAsync(downloadCopy);
                    downloadId = downloadCopy.Id;
                }

                if (tvChannel.HasSampleDownload)
                {
                    var sampleDownload = await _downloadService.GetDownloadByIdAsync(tvChannel.SampleDownloadId);
                    if (sampleDownload != null)
                    {
                        var sampleDownloadCopy = new Download
                        {
                            DownloadGuid = Guid.NewGuid(),
                            UseDownloadUrl = sampleDownload.UseDownloadUrl,
                            DownloadUrl = sampleDownload.DownloadUrl,
                            DownloadBinary = sampleDownload.DownloadBinary,
                            ContentType = sampleDownload.ContentType,
                            Filename = sampleDownload.Filename,
                            Extension = sampleDownload.Extension,
                            IsNew = sampleDownload.IsNew
                        };
                        await _downloadService.InsertDownloadAsync(sampleDownloadCopy);
                        sampleDownloadId = sampleDownloadCopy.Id;
                    }
                }
            }

            var newSku = !string.IsNullOrWhiteSpace(tvChannel.Sku)
                ? string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Copy.SKU.New"), tvChannel.Sku)
                : tvChannel.Sku;
            // tvChannel
            var tvChannelCopy = new TvChannel
            {
                TvChannelTypeId = tvChannel.TvChannelTypeId,
                ParentGroupedTvChannelId = tvChannel.ParentGroupedTvChannelId,
                VisibleIndividually = tvChannel.VisibleIndividually,
                Name = newName,
                ShortDescription = tvChannel.ShortDescription,
                FullDescription = tvChannel.FullDescription,
                VendorId = tvChannel.VendorId,
                TvChannelTemplateId = tvChannel.TvChannelTemplateId,
                AdminComment = tvChannel.AdminComment,
                ShowOnHomepage = tvChannel.ShowOnHomepage,
                MetaKeywords = tvChannel.MetaKeywords,
                MetaDescription = tvChannel.MetaDescription,
                MetaTitle = tvChannel.MetaTitle,
                AllowUserReviews = tvChannel.AllowUserReviews,
                LimitedToStores = tvChannel.LimitedToStores,
                SubjectToAcl = tvChannel.SubjectToAcl,
                Sku = newSku,
                ManufacturerPartNumber = tvChannel.ManufacturerPartNumber,
                Gtin = tvChannel.Gtin,
                IsGiftCard = tvChannel.IsGiftCard,
                GiftCardType = tvChannel.GiftCardType,
                OverriddenGiftCardAmount = tvChannel.OverriddenGiftCardAmount,
                RequireOtherTvChannels = tvChannel.RequireOtherTvChannels,
                RequiredTvChannelIds = tvChannel.RequiredTvChannelIds,
                AutomaticallyAddRequiredTvChannels = tvChannel.AutomaticallyAddRequiredTvChannels,
                IsDownload = tvChannel.IsDownload,
                DownloadId = downloadId,
                UnlimitedDownloads = tvChannel.UnlimitedDownloads,
                MaxNumberOfDownloads = tvChannel.MaxNumberOfDownloads,
                DownloadExpirationDays = tvChannel.DownloadExpirationDays,
                DownloadActivationType = tvChannel.DownloadActivationType,
                HasSampleDownload = tvChannel.HasSampleDownload,
                SampleDownloadId = sampleDownloadId,
                HasUserAgreement = tvChannel.HasUserAgreement,
                UserAgreementText = tvChannel.UserAgreementText,
                IsRecurring = tvChannel.IsRecurring,
                RecurringCycleLength = tvChannel.RecurringCycleLength,
                RecurringCyclePeriod = tvChannel.RecurringCyclePeriod,
                RecurringTotalCycles = tvChannel.RecurringTotalCycles,
                IsRental = tvChannel.IsRental,
                RentalPriceLength = tvChannel.RentalPriceLength,
                RentalPricePeriod = tvChannel.RentalPricePeriod,
                IsShipEnabled = tvChannel.IsShipEnabled,
                IsFreeShipping = tvChannel.IsFreeShipping,
                ShipSeparately = tvChannel.ShipSeparately,
                AdditionalShippingCharge = tvChannel.AdditionalShippingCharge,
                DeliveryDateId = tvChannel.DeliveryDateId,
                IsTaxExempt = tvChannel.IsTaxExempt,
                TaxCategoryId = tvChannel.TaxCategoryId,
                IsTelecommunicationsOrBroadcastingOrElectronicServices =
                    tvChannel.IsTelecommunicationsOrBroadcastingOrElectronicServices,
                ManageInventoryMethod = tvChannel.ManageInventoryMethod,
                TvChannelAvailabilityRangeId = tvChannel.TvChannelAvailabilityRangeId,
                UseMultipleWarehouses = tvChannel.UseMultipleWarehouses,
                WarehouseId = tvChannel.WarehouseId,
                StockQuantity = tvChannel.StockQuantity,
                DisplayStockAvailability = tvChannel.DisplayStockAvailability,
                DisplayStockQuantity = tvChannel.DisplayStockQuantity,
                MinStockQuantity = tvChannel.MinStockQuantity,
                LowStockActivityId = tvChannel.LowStockActivityId,
                NotifyAdminForQuantityBelow = tvChannel.NotifyAdminForQuantityBelow,
                BackorderMode = tvChannel.BackorderMode,
                AllowBackInStockSubscriptions = tvChannel.AllowBackInStockSubscriptions,
                OrderMinimumQuantity = tvChannel.OrderMinimumQuantity,
                OrderMaximumQuantity = tvChannel.OrderMaximumQuantity,
                AllowedQuantities = tvChannel.AllowedQuantities,
                AllowAddingOnlyExistingAttributeCombinations = tvChannel.AllowAddingOnlyExistingAttributeCombinations,
                NotReturnable = tvChannel.NotReturnable,
                DisableBuyButton = tvChannel.DisableBuyButton,
                DisableWishlistButton = tvChannel.DisableWishlistButton,
                AvailableForPreOrder = tvChannel.AvailableForPreOrder,
                PreOrderAvailabilityStartDateTimeUtc = tvChannel.PreOrderAvailabilityStartDateTimeUtc,
                CallForPrice = tvChannel.CallForPrice,
                Price = tvChannel.Price,
                OldPrice = tvChannel.OldPrice,
                TvChannelCost = tvChannel.TvChannelCost,
                UserEntersPrice = tvChannel.UserEntersPrice,
                MinimumUserEnteredPrice = tvChannel.MinimumUserEnteredPrice,
                MaximumUserEnteredPrice = tvChannel.MaximumUserEnteredPrice,
                BasepriceEnabled = tvChannel.BasepriceEnabled,
                BasepriceAmount = tvChannel.BasepriceAmount,
                BasepriceUnitId = tvChannel.BasepriceUnitId,
                BasepriceBaseAmount = tvChannel.BasepriceBaseAmount,
                BasepriceBaseUnitId = tvChannel.BasepriceBaseUnitId,
                MarkAsNew = tvChannel.MarkAsNew,
                MarkAsNewStartDateTimeUtc = tvChannel.MarkAsNewStartDateTimeUtc,
                MarkAsNewEndDateTimeUtc = tvChannel.MarkAsNewEndDateTimeUtc,
                Weight = tvChannel.Weight,
                Length = tvChannel.Length,
                Width = tvChannel.Width,
                Height = tvChannel.Height,
                AvailableStartDateTimeUtc = tvChannel.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = tvChannel.AvailableEndDateTimeUtc,
                DisplayOrder = tvChannel.DisplayOrder,
                Published = isPublished,
                Deleted = tvChannel.Deleted,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            //validate search engine name
            await _tvChannelService.InsertTvChannelAsync(tvChannelCopy);

            //search engine name
            await _urlRecordService.SaveSlugAsync(tvChannelCopy, await _urlRecordService.ValidateSeNameAsync(tvChannelCopy, string.Empty, tvChannelCopy.Name, true), 0);
            return tvChannelCopy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a copy of tvChannel with all depended data
        /// </summary>
        /// <param name="tvChannel">The tvChannel to copy</param>
        /// <param name="newName">The name of tvChannel duplicate</param>
        /// <param name="isPublished">A value indicating whether the tvChannel duplicate should be published</param>
        /// <param name="copyMultimedia">A value indicating whether the tvChannel images and videos should be copied</param>
        /// <param name="copyAssociatedTvChannels">A value indicating whether the copy associated tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel copy
        /// </returns>
        public virtual async Task<TvChannel> CopyTvChannelAsync(TvChannel tvChannel, string newName,
            bool isPublished = true, bool copyMultimedia = true, bool copyAssociatedTvChannels = true)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException("TvChannel name is required");

            var tvChannelCopy = await CopyBaseTvChannelDataAsync(tvChannel, newName, isPublished);

            //localization
            await CopyLocalizationDataAsync(tvChannel, tvChannelCopy);

            //copy tvChannel tags
            foreach (var tvChannelTag in await _tvChannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvChannel.Id)) 
                await _tvChannelTagService.InsertTvChannelTvChannelTagMappingAsync(new TvChannelTvChannelTagMapping { TvChannelTagId = tvChannelTag.Id, TvChannelId = tvChannelCopy.Id });

            await _tvChannelService.UpdateTvChannelAsync(tvChannelCopy);

            //copy tvChannel pictures
            var originalNewPictureIdentifiers = await CopyTvChannelPicturesAsync(tvChannel, newName, copyMultimedia, tvChannelCopy);

            //copy tvChannel videos
            await CopyTvChannelVideosAsync(tvChannel, copyMultimedia, tvChannelCopy);

            //quantity change history
            await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannelCopy, tvChannel.StockQuantity, tvChannel.StockQuantity, tvChannel.WarehouseId,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyTvChannel"), tvChannel.Id));

            //tvChannel specifications
            await CopyTvChannelSpecificationsAsync(tvChannel, tvChannelCopy);

            //tvChannel <-> warehouses mappings
            await CopyWarehousesMappingAsync(tvChannel, tvChannelCopy);
            //tvChannel <-> categories mappings
            await CopyCategoriesMappingAsync(tvChannel, tvChannelCopy);
            //tvChannel <-> manufacturers mappings
            await CopyManufacturersMappingAsync(tvChannel, tvChannelCopy);
            //tvChannel <-> related tvChannels mappings
            await CopyRelatedTvChannelsMappingAsync(tvChannel, tvChannelCopy);
            //tvChannel <-> cross sells mappings
            await CopyCrossSellsMappingAsync(tvChannel, tvChannelCopy);
            //tvChannel <-> attributes mappings
            await CopyAttributesMappingAsync(tvChannel, tvChannelCopy, originalNewPictureIdentifiers);
            //tvChannel <-> discounts mapping
            await CopyDiscountsMappingAsync(tvChannel, tvChannelCopy);

            //store mapping
            var selectedStoreIds = await _storeMappingService.GetStoresIdsWithAccessAsync(tvChannel);
            foreach (var id in selectedStoreIds) 
                await _storeMappingService.InsertStoreMappingAsync(tvChannelCopy, id);

            //user role mapping
            var userRoleIds = await _aclService.GetUserRoleIdsWithAccessAsync(tvChannel);
            foreach (var id in userRoleIds)
                await _aclService.InsertAclRecordAsync(tvChannelCopy, id);

            //tier prices
            await CopyTierPricesAsync(tvChannel, tvChannelCopy);

            //update "HasTierPrices" and "HasDiscountsApplied" properties
            await _tvChannelService.UpdateHasTierPricesPropertyAsync(tvChannelCopy);
            await _tvChannelService.UpdateHasDiscountsAppliedAsync(tvChannelCopy);

            //associated tvChannels
            await CopyAssociatedTvChannelsAsync(tvChannel, isPublished, copyMultimedia, copyAssociatedTvChannels, tvChannelCopy);

            return tvChannelCopy;
        }

        #endregion
    }
}