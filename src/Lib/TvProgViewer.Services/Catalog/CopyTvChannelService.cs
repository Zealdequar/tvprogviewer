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
        private readonly ITvChannelAttributeParser _tvchannelAttributeParser;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;
        private readonly ITvChannelService _tvchannelService;
        private readonly ITvChannelTagService _tvchannelTagService;
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
            ITvChannelAttributeParser tvchannelAttributeParser,
            ITvChannelAttributeService tvchannelAttributeService,
            ITvChannelService tvchannelService,
            ITvChannelTagService tvchannelTagService,
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
            _tvchannelAttributeParser = tvchannelAttributeParser;
            _tvchannelAttributeService = tvchannelAttributeService;
            _tvchannelService = tvchannelService;
            _tvchannelTagService = tvchannelTagService;
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
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyDiscountsMappingAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            foreach (var discountMapping in await _tvchannelService.GetAllDiscountsAppliedToTvChannelAsync(tvchannel.Id))
            {
                await _tvchannelService.InsertDiscountTvChannelMappingAsync(new DiscountTvChannelMapping { EntityId = tvchannelCopy.Id, DiscountId = discountMapping.DiscountId });
                await _tvchannelService.UpdateTvChannelAsync(tvchannelCopy);
            }
        }

        /// <summary>
        /// Copy associated tvchannels
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="isPublished">A value indicating whether they should be published</param>
        /// <param name="copyMultimedia">A value indicating whether to copy images and videos</param>
        /// <param name="copyAssociatedTvChannels">A value indicating whether to copy associated tvchannels</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyAssociatedTvChannelsAsync(TvChannel tvchannel, bool isPublished, bool copyMultimedia, bool copyAssociatedTvChannels, TvChannel tvchannelCopy)
        {
            if (!copyAssociatedTvChannels)
                return;

            var associatedTvChannels = await _tvchannelService.GetAssociatedTvChannelsAsync(tvchannel.Id, showHidden: true);
            foreach (var associatedTvChannel in associatedTvChannels)
            {
                var associatedTvChannelCopy = await CopyTvChannelAsync(associatedTvChannel,
                    string.Format(TvProgCatalogDefaults.TvChannelCopyNameTemplate, associatedTvChannel.Name),
                    isPublished, copyMultimedia, false);
                associatedTvChannelCopy.ParentGroupedTvChannelId = tvchannelCopy.Id;
                await _tvchannelService.UpdateTvChannelAsync(associatedTvChannelCopy);
            }
        }

        /// <summary>
        /// Copy tier prices
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyTierPricesAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            foreach (var tierPrice in await _tvchannelService.GetTierPricesByTvChannelAsync(tvchannel.Id))
                await _tvchannelService.InsertTierPriceAsync(new TierPrice
                {
                    TvChannelId = tvchannelCopy.Id,
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
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <param name="originalNewPictureIdentifiers">Identifiers of pictures</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyAttributesMappingAsync(TvChannel tvchannel, TvChannel tvchannelCopy, Dictionary<int, int> originalNewPictureIdentifiers)
        {
            var associatedAttributes = new Dictionary<int, int>();
            var associatedAttributeValues = new Dictionary<int, int>();

            //attribute mapping with condition attributes
            var oldCopyWithConditionAttributes = new List<TvChannelAttributeMapping>();

            //all tvchannel attribute mapping copies
            var tvchannelAttributeMappingCopies = new Dictionary<int, TvChannelAttributeMapping>();

            var languages = await _languageService.GetAllLanguagesAsync(true);

            foreach (var tvchannelAttributeMapping in await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id))
            {
                var tvchannelAttributeMappingCopy = new TvChannelAttributeMapping
                {
                    TvChannelId = tvchannelCopy.Id,
                    TvChannelAttributeId = tvchannelAttributeMapping.TvChannelAttributeId,
                    TextPrompt = tvchannelAttributeMapping.TextPrompt,
                    IsRequired = tvchannelAttributeMapping.IsRequired,
                    AttributeControlTypeId = tvchannelAttributeMapping.AttributeControlTypeId,
                    DisplayOrder = tvchannelAttributeMapping.DisplayOrder,
                    ValidationMinLength = tvchannelAttributeMapping.ValidationMinLength,
                    ValidationMaxLength = tvchannelAttributeMapping.ValidationMaxLength,
                    ValidationFileAllowedExtensions = tvchannelAttributeMapping.ValidationFileAllowedExtensions,
                    ValidationFileMaximumSize = tvchannelAttributeMapping.ValidationFileMaximumSize,
                    DefaultValue = tvchannelAttributeMapping.DefaultValue
                };
                await _tvchannelAttributeService.InsertTvChannelAttributeMappingAsync(tvchannelAttributeMappingCopy);
                //localization
                foreach (var lang in languages)
                {
                    var textPrompt = await _localizationService.GetLocalizedAsync(tvchannelAttributeMapping, x => x.TextPrompt, lang.Id, false, false);
                    if (!string.IsNullOrEmpty(textPrompt))
                        await _localizedEntityService.SaveLocalizedValueAsync(tvchannelAttributeMappingCopy, x => x.TextPrompt, textPrompt,
                            lang.Id);
                }

                tvchannelAttributeMappingCopies.Add(tvchannelAttributeMappingCopy.Id, tvchannelAttributeMappingCopy);

                if (!string.IsNullOrEmpty(tvchannelAttributeMapping.ConditionAttributeXml))
                {
                    oldCopyWithConditionAttributes.Add(tvchannelAttributeMapping);
                }

                //save associated value (used for combinations copying)
                associatedAttributes.Add(tvchannelAttributeMapping.Id, tvchannelAttributeMappingCopy.Id);

                // tvchannel attribute values
                var tvchannelAttributeValues = await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(tvchannelAttributeMapping.Id);
                foreach (var tvchannelAttributeValue in tvchannelAttributeValues)
                {
                    var attributeValuePictureId = 0;
                    if (originalNewPictureIdentifiers.ContainsKey(tvchannelAttributeValue.PictureId)) 
                        attributeValuePictureId = originalNewPictureIdentifiers[tvchannelAttributeValue.PictureId];

                    var attributeValueCopy = new TvChannelAttributeValue
                    {
                        TvChannelAttributeMappingId = tvchannelAttributeMappingCopy.Id,
                        AttributeValueTypeId = tvchannelAttributeValue.AttributeValueTypeId,
                        AssociatedTvChannelId = tvchannelAttributeValue.AssociatedTvChannelId,
                        Name = tvchannelAttributeValue.Name,
                        ColorSquaresRgb = tvchannelAttributeValue.ColorSquaresRgb,
                        PriceAdjustment = tvchannelAttributeValue.PriceAdjustment,
                        PriceAdjustmentUsePercentage = tvchannelAttributeValue.PriceAdjustmentUsePercentage,
                        WeightAdjustment = tvchannelAttributeValue.WeightAdjustment,
                        Cost = tvchannelAttributeValue.Cost,
                        UserEntersQty = tvchannelAttributeValue.UserEntersQty,
                        Quantity = tvchannelAttributeValue.Quantity,
                        IsPreSelected = tvchannelAttributeValue.IsPreSelected,
                        DisplayOrder = tvchannelAttributeValue.DisplayOrder,
                        PictureId = attributeValuePictureId,
                    };
                    //picture associated to "iamge square" attribute type (if exists)
                    if (tvchannelAttributeValue.ImageSquaresPictureId > 0)
                    {
                        var origImageSquaresPicture =
                            await _pictureService.GetPictureByIdAsync(tvchannelAttributeValue.ImageSquaresPictureId);
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

                    await _tvchannelAttributeService.InsertTvChannelAttributeValueAsync(attributeValueCopy);

                    //save associated value (used for combinations copying)
                    associatedAttributeValues.Add(tvchannelAttributeValue.Id, attributeValueCopy.Id);

                    //localization
                    foreach (var lang in languages)
                    {
                        var name = await _localizationService.GetLocalizedAsync(tvchannelAttributeValue, x => x.Name, lang.Id, false, false);
                        if (!string.IsNullOrEmpty(name))
                            await _localizedEntityService.SaveLocalizedValueAsync(attributeValueCopy, x => x.Name, name, lang.Id);
                    }
                }
            }

            //copy attribute conditions
            foreach (var tvchannelAttributeMapping in oldCopyWithConditionAttributes)
            {
                var oldConditionAttributeMapping = (await _tvchannelAttributeParser
                    .ParseTvChannelAttributeMappingsAsync(tvchannelAttributeMapping.ConditionAttributeXml)).FirstOrDefault();

                if (oldConditionAttributeMapping == null)
                    continue;

                var oldConditionValues = await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(
                    tvchannelAttributeMapping.ConditionAttributeXml,
                    oldConditionAttributeMapping.Id);

                if (!oldConditionValues.Any())
                    continue;

                var newAttributeMappingId = associatedAttributes[oldConditionAttributeMapping.Id];
                var newConditionAttributeMapping = tvchannelAttributeMappingCopies[newAttributeMappingId];

                var newConditionAttributeXml = string.Empty;

                foreach (var oldConditionValue in oldConditionValues)
                {
                    newConditionAttributeXml = _tvchannelAttributeParser.AddTvChannelAttribute(newConditionAttributeXml,
                        newConditionAttributeMapping, associatedAttributeValues[oldConditionValue.Id].ToString());
                }

                var attributeMappingId = associatedAttributes[tvchannelAttributeMapping.Id];
                var conditionAttribute = tvchannelAttributeMappingCopies[attributeMappingId];
                conditionAttribute.ConditionAttributeXml = newConditionAttributeXml;

                await _tvchannelAttributeService.UpdateTvChannelAttributeMappingAsync(conditionAttribute);
            }

            //attribute combinations
            foreach (var combination in await _tvchannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id))
            {
                //generate new AttributesXml according to new value IDs
                var newAttributesXml = string.Empty;
                var parsedTvChannelAttributes = await _tvchannelAttributeParser.ParseTvChannelAttributeMappingsAsync(combination.AttributesXml);
                foreach (var oldAttribute in parsedTvChannelAttributes)
                {
                    if (!associatedAttributes.ContainsKey(oldAttribute.Id))
                        continue;

                    var newAttribute = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(associatedAttributes[oldAttribute.Id]);

                    if (newAttribute == null)
                        continue;

                    var oldAttributeValuesStr = _tvchannelAttributeParser.ParseValues(combination.AttributesXml, oldAttribute.Id);

                    foreach (var oldAttributeValueStr in oldAttributeValuesStr)
                    {
                        if (newAttribute.ShouldHaveValues())
                        {
                            //attribute values
                            var oldAttributeValue = int.Parse(oldAttributeValueStr);
                            if (!associatedAttributeValues.ContainsKey(oldAttributeValue))
                                continue;

                            var newAttributeValue = await _tvchannelAttributeService.GetTvChannelAttributeValueByIdAsync(associatedAttributeValues[oldAttributeValue]);

                            if (newAttributeValue != null)
                            {
                                newAttributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(newAttributesXml,
                                    newAttribute, newAttributeValue.Id.ToString());
                            }
                        }
                        else
                        {
                            //just a text
                            newAttributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(newAttributesXml,
                                newAttribute, oldAttributeValueStr);
                        }
                    }
                }

                //picture
                originalNewPictureIdentifiers.TryGetValue(combination.PictureId, out var combinationPictureId);

                var combinationCopy = new TvChannelAttributeCombination
                {
                    TvChannelId = tvchannelCopy.Id,
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
                await _tvchannelAttributeService.InsertTvChannelAttributeCombinationAsync(combinationCopy);

                //quantity change history
                await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannelCopy, combination.StockQuantity,
                    combination.StockQuantity,
                    message: string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyTvChannel"), tvchannel.Id), combinationId: combination.Id);
            }
        }

        /// <summary>
        /// Copy tvchannel specifications
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyTvChannelSpecificationsAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync();

            foreach (var tvchannelSpecificationAttribute in await _specificationAttributeService.GetTvChannelSpecificationAttributesAsync(tvchannel.Id))
            {
                var psaCopy = new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvchannelCopy.Id,
                    AttributeTypeId = tvchannelSpecificationAttribute.AttributeTypeId,
                    SpecificationAttributeOptionId = tvchannelSpecificationAttribute.SpecificationAttributeOptionId,
                    CustomValue = tvchannelSpecificationAttribute.CustomValue,
                    AllowFiltering = tvchannelSpecificationAttribute.AllowFiltering,
                    ShowOnTvChannelPage = tvchannelSpecificationAttribute.ShowOnTvChannelPage,
                    DisplayOrder = tvchannelSpecificationAttribute.DisplayOrder
                };

                await _specificationAttributeService.InsertTvChannelSpecificationAttributeAsync(psaCopy);
                
                foreach (var language in allLanguages)
                {
                    var customValue = await _localizationService.GetLocalizedAsync(tvchannelSpecificationAttribute, x => x.CustomValue, language.Id, false, false);
                    if (!string.IsNullOrEmpty(customValue))
                        await _localizedEntityService.SaveLocalizedValueAsync(psaCopy, x => x.CustomValue, customValue, language.Id);
                }
            }
        }

        /// <summary>
        /// Copy crosssell mapping
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyCrossSellsMappingAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            foreach (var csTvChannel in await _tvchannelService.GetCrossSellTvChannelsByTvChannelId1Async(tvchannel.Id, true))
                await _tvchannelService.InsertCrossSellTvChannelAsync(
                    new CrossSellTvChannel
                    {
                        TvChannelId1 = tvchannelCopy.Id,
                        TvChannelId2 = csTvChannel.TvChannelId2
                    });
        }

        /// <summary>
        /// Copy related tvchannels mapping
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyRelatedTvChannelsMappingAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            foreach (var relatedTvChannel in await _tvchannelService.GetRelatedTvChannelsByTvChannelId1Async(tvchannel.Id, true))
                await _tvchannelService.InsertRelatedTvChannelAsync(
                    new RelatedTvChannel
                    {
                        TvChannelId1 = tvchannelCopy.Id,
                        TvChannelId2 = relatedTvChannel.TvChannelId2,
                        DisplayOrder = relatedTvChannel.DisplayOrder
                    });
        }

        /// <summary>
        /// Copy manufacturer mapping
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyManufacturersMappingAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            foreach (var tvchannelManufacturers in await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvchannel.Id, true))
            {
                var tvchannelManufacturerCopy = new TvChannelManufacturer
                {
                    TvChannelId = tvchannelCopy.Id,
                    ManufacturerId = tvchannelManufacturers.ManufacturerId,
                    IsFeaturedTvChannel = tvchannelManufacturers.IsFeaturedTvChannel,
                    DisplayOrder = tvchannelManufacturers.DisplayOrder
                };

                await _manufacturerService.InsertTvChannelManufacturerAsync(tvchannelManufacturerCopy);
            }
        }

        /// <summary>
        /// Copy category mapping
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyCategoriesMappingAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            foreach (var tvchannelCategory in await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvchannel.Id, showHidden: true))
            {
                var tvchannelCategoryCopy = new TvChannelCategory
                {
                    TvChannelId = tvchannelCopy.Id,
                    CategoryId = tvchannelCategory.CategoryId,
                    IsFeaturedTvChannel = tvchannelCategory.IsFeaturedTvChannel,
                    DisplayOrder = tvchannelCategory.DisplayOrder
                };

                await _categoryService.InsertTvChannelCategoryAsync(tvchannelCategoryCopy);
            }
        }

        /// <summary>
        /// Copy warehouse mapping
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyWarehousesMappingAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            foreach (var pwi in await _tvchannelService.GetAllTvChannelWarehouseInventoryRecordsAsync(tvchannel.Id))
            {
                await _tvchannelService.InsertTvChannelWarehouseInventoryAsync(
                    new TvChannelWarehouseInventory
                    {
                        TvChannelId = tvchannelCopy.Id,
                        WarehouseId = pwi.WarehouseId,
                        StockQuantity = pwi.StockQuantity,
                        ReservedQuantity = 0
                    });

                //quantity change history
                var message = $"{await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyTvChannel"), tvchannel.Id)}";
                await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannelCopy, pwi.StockQuantity, pwi.StockQuantity, pwi.WarehouseId, message);
            }

            await _tvchannelService.UpdateTvChannelAsync(tvchannelCopy);
        }

        /// <summary>
        /// Copy tvchannel pictures
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="newName">New tvchannel name</param>
        /// <param name="copyMultimedia"></param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the identifiers of old and new pictures
        /// </returns>
        protected virtual async Task<Dictionary<int, int>> CopyTvChannelPicturesAsync(TvChannel tvchannel, string newName, bool copyMultimedia, TvChannel tvchannelCopy)
        {
            //variable to store original and new picture identifiers
            var originalNewPictureIdentifiers = new Dictionary<int, int>();
            if (!copyMultimedia)
                return originalNewPictureIdentifiers;

            foreach (var tvchannelPicture in await _tvchannelService.GetTvChannelPicturesByTvChannelIdAsync(tvchannel.Id))
            {
                var picture = await _pictureService.GetPictureByIdAsync(tvchannelPicture.PictureId);
                var pictureCopy = await _pictureService.InsertPictureAsync(
                    await _pictureService.LoadPictureBinaryAsync(picture),
                    picture.MimeType,
                    await _pictureService.GetPictureSeNameAsync(newName),
                    picture.AltAttribute,
                    picture.TitleAttribute);
                await _tvchannelService.InsertTvChannelPictureAsync(new TvChannelPicture
                {
                    TvChannelId = tvchannelCopy.Id,
                    PictureId = pictureCopy.Id,
                    DisplayOrder = tvchannelPicture.DisplayOrder
                });
                originalNewPictureIdentifiers.Add(picture.Id, pictureCopy.Id);
            }

            return originalNewPictureIdentifiers;
        }

        /// <summary>
        /// Copy tvchannel videos
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="copyVideos"></param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyTvChannelVideosAsync(TvChannel tvchannel, bool copyVideos, TvChannel tvchannelCopy)
        {
            if (copyVideos)
            {
                foreach (var tvchannelVideo in await _tvchannelService.GetTvChannelVideosByTvChannelIdAsync(tvchannel.Id))
                {
                    var video = await _videoService.GetVideoByIdAsync(tvchannelVideo.VideoId);
                    var videoCopy = await _videoService.InsertVideoAsync(video);
                    await _tvchannelService.InsertTvChannelVideoAsync(new TvChannelVideo
                    {
                        TvChannelId = tvchannelCopy.Id,
                        VideoId = videoCopy.Id,
                        DisplayOrder = tvchannelVideo.DisplayOrder
                    });
                }
            }
        }

        /// <summary>
        /// Copy localization data
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelCopy">New tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task CopyLocalizationDataAsync(TvChannel tvchannel, TvChannel tvchannelCopy)
        {
            var languages = await _languageService.GetAllLanguagesAsync(true);

            //localization
            foreach (var lang in languages)
            {
                var name = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name, lang.Id, false, false);
                if (!string.IsNullOrEmpty(name))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvchannelCopy, x => x.Name, name, lang.Id);

                var shortDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.ShortDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(shortDescription))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvchannelCopy, x => x.ShortDescription, shortDescription, lang.Id);

                var fullDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.FullDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(fullDescription))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvchannelCopy, x => x.FullDescription, fullDescription, lang.Id);

                var metaKeywords = await _localizationService.GetLocalizedAsync(tvchannel, x => x.MetaKeywords, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaKeywords))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvchannelCopy, x => x.MetaKeywords, metaKeywords, lang.Id);

                var metaDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.MetaDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaDescription))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvchannelCopy, x => x.MetaDescription, metaDescription, lang.Id);

                var metaTitle = await _localizationService.GetLocalizedAsync(tvchannel, x => x.MetaTitle, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaTitle))
                    await _localizedEntityService.SaveLocalizedValueAsync(tvchannelCopy, x => x.MetaTitle, metaTitle, lang.Id);

                //search engine name
                await _urlRecordService.SaveSlugAsync(tvchannelCopy, await _urlRecordService.ValidateSeNameAsync(tvchannelCopy, string.Empty, name, false), lang.Id);
            }
        }

        /// <summary>
        /// Copy tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="newName">New tvchannel name</param>
        /// <param name="isPublished">A value indicating whether a new tvchannel is published</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        protected virtual async Task<TvChannel> CopyBaseTvChannelDataAsync(TvChannel tvchannel, string newName, bool isPublished)
        {
            //tvchannel download & sample download
            var downloadId = tvchannel.DownloadId;
            var sampleDownloadId = tvchannel.SampleDownloadId;
            if (tvchannel.IsDownload)
            {
                var download = await _downloadService.GetDownloadByIdAsync(tvchannel.DownloadId);
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

                if (tvchannel.HasSampleDownload)
                {
                    var sampleDownload = await _downloadService.GetDownloadByIdAsync(tvchannel.SampleDownloadId);
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

            var newSku = !string.IsNullOrWhiteSpace(tvchannel.Sku)
                ? string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Copy.SKU.New"), tvchannel.Sku)
                : tvchannel.Sku;
            // tvchannel
            var tvchannelCopy = new TvChannel
            {
                TvChannelTypeId = tvchannel.TvChannelTypeId,
                ParentGroupedTvChannelId = tvchannel.ParentGroupedTvChannelId,
                VisibleIndividually = tvchannel.VisibleIndividually,
                Name = newName,
                ShortDescription = tvchannel.ShortDescription,
                FullDescription = tvchannel.FullDescription,
                VendorId = tvchannel.VendorId,
                TvChannelTemplateId = tvchannel.TvChannelTemplateId,
                AdminComment = tvchannel.AdminComment,
                ShowOnHomepage = tvchannel.ShowOnHomepage,
                MetaKeywords = tvchannel.MetaKeywords,
                MetaDescription = tvchannel.MetaDescription,
                MetaTitle = tvchannel.MetaTitle,
                AllowUserReviews = tvchannel.AllowUserReviews,
                LimitedToStores = tvchannel.LimitedToStores,
                SubjectToAcl = tvchannel.SubjectToAcl,
                Sku = newSku,
                ManufacturerPartNumber = tvchannel.ManufacturerPartNumber,
                Gtin = tvchannel.Gtin,
                IsGiftCard = tvchannel.IsGiftCard,
                GiftCardType = tvchannel.GiftCardType,
                OverriddenGiftCardAmount = tvchannel.OverriddenGiftCardAmount,
                RequireOtherTvChannels = tvchannel.RequireOtherTvChannels,
                RequiredTvChannelIds = tvchannel.RequiredTvChannelIds,
                AutomaticallyAddRequiredTvChannels = tvchannel.AutomaticallyAddRequiredTvChannels,
                IsDownload = tvchannel.IsDownload,
                DownloadId = downloadId,
                UnlimitedDownloads = tvchannel.UnlimitedDownloads,
                MaxNumberOfDownloads = tvchannel.MaxNumberOfDownloads,
                DownloadExpirationDays = tvchannel.DownloadExpirationDays,
                DownloadActivationType = tvchannel.DownloadActivationType,
                HasSampleDownload = tvchannel.HasSampleDownload,
                SampleDownloadId = sampleDownloadId,
                HasUserAgreement = tvchannel.HasUserAgreement,
                UserAgreementText = tvchannel.UserAgreementText,
                IsRecurring = tvchannel.IsRecurring,
                RecurringCycleLength = tvchannel.RecurringCycleLength,
                RecurringCyclePeriod = tvchannel.RecurringCyclePeriod,
                RecurringTotalCycles = tvchannel.RecurringTotalCycles,
                IsRental = tvchannel.IsRental,
                RentalPriceLength = tvchannel.RentalPriceLength,
                RentalPricePeriod = tvchannel.RentalPricePeriod,
                IsShipEnabled = tvchannel.IsShipEnabled,
                IsFreeShipping = tvchannel.IsFreeShipping,
                ShipSeparately = tvchannel.ShipSeparately,
                AdditionalShippingCharge = tvchannel.AdditionalShippingCharge,
                DeliveryDateId = tvchannel.DeliveryDateId,
                IsTaxExempt = tvchannel.IsTaxExempt,
                TaxCategoryId = tvchannel.TaxCategoryId,
                IsTelecommunicationsOrBroadcastingOrElectronicServices =
                    tvchannel.IsTelecommunicationsOrBroadcastingOrElectronicServices,
                ManageInventoryMethod = tvchannel.ManageInventoryMethod,
                TvChannelAvailabilityRangeId = tvchannel.TvChannelAvailabilityRangeId,
                UseMultipleWarehouses = tvchannel.UseMultipleWarehouses,
                WarehouseId = tvchannel.WarehouseId,
                StockQuantity = tvchannel.StockQuantity,
                DisplayStockAvailability = tvchannel.DisplayStockAvailability,
                DisplayStockQuantity = tvchannel.DisplayStockQuantity,
                MinStockQuantity = tvchannel.MinStockQuantity,
                LowStockActivityId = tvchannel.LowStockActivityId,
                NotifyAdminForQuantityBelow = tvchannel.NotifyAdminForQuantityBelow,
                BackorderMode = tvchannel.BackorderMode,
                AllowBackInStockSubscriptions = tvchannel.AllowBackInStockSubscriptions,
                OrderMinimumQuantity = tvchannel.OrderMinimumQuantity,
                OrderMaximumQuantity = tvchannel.OrderMaximumQuantity,
                AllowedQuantities = tvchannel.AllowedQuantities,
                AllowAddingOnlyExistingAttributeCombinations = tvchannel.AllowAddingOnlyExistingAttributeCombinations,
                NotReturnable = tvchannel.NotReturnable,
                DisableBuyButton = tvchannel.DisableBuyButton,
                DisableWishlistButton = tvchannel.DisableWishlistButton,
                AvailableForPreOrder = tvchannel.AvailableForPreOrder,
                PreOrderAvailabilityStartDateTimeUtc = tvchannel.PreOrderAvailabilityStartDateTimeUtc,
                CallForPrice = tvchannel.CallForPrice,
                Price = tvchannel.Price,
                OldPrice = tvchannel.OldPrice,
                TvChannelCost = tvchannel.TvChannelCost,
                UserEntersPrice = tvchannel.UserEntersPrice,
                MinimumUserEnteredPrice = tvchannel.MinimumUserEnteredPrice,
                MaximumUserEnteredPrice = tvchannel.MaximumUserEnteredPrice,
                BasepriceEnabled = tvchannel.BasepriceEnabled,
                BasepriceAmount = tvchannel.BasepriceAmount,
                BasepriceUnitId = tvchannel.BasepriceUnitId,
                BasepriceBaseAmount = tvchannel.BasepriceBaseAmount,
                BasepriceBaseUnitId = tvchannel.BasepriceBaseUnitId,
                MarkAsNew = tvchannel.MarkAsNew,
                MarkAsNewStartDateTimeUtc = tvchannel.MarkAsNewStartDateTimeUtc,
                MarkAsNewEndDateTimeUtc = tvchannel.MarkAsNewEndDateTimeUtc,
                Weight = tvchannel.Weight,
                Length = tvchannel.Length,
                Width = tvchannel.Width,
                Height = tvchannel.Height,
                AvailableStartDateTimeUtc = tvchannel.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = tvchannel.AvailableEndDateTimeUtc,
                DisplayOrder = tvchannel.DisplayOrder,
                Published = isPublished,
                Deleted = tvchannel.Deleted,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            //validate search engine name
            await _tvchannelService.InsertTvChannelAsync(tvchannelCopy);

            //search engine name
            await _urlRecordService.SaveSlugAsync(tvchannelCopy, await _urlRecordService.ValidateSeNameAsync(tvchannelCopy, string.Empty, tvchannelCopy.Name, true), 0);
            return tvchannelCopy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a copy of tvchannel with all depended data
        /// </summary>
        /// <param name="tvchannel">The tvchannel to copy</param>
        /// <param name="newName">The name of tvchannel duplicate</param>
        /// <param name="isPublished">A value indicating whether the tvchannel duplicate should be published</param>
        /// <param name="copyMultimedia">A value indicating whether the tvchannel images and videos should be copied</param>
        /// <param name="copyAssociatedTvChannels">A value indicating whether the copy associated tvchannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel copy
        /// </returns>
        public virtual async Task<TvChannel> CopyTvChannelAsync(TvChannel tvchannel, string newName,
            bool isPublished = true, bool copyMultimedia = true, bool copyAssociatedTvChannels = true)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException("TvChannel name is required");

            var tvchannelCopy = await CopyBaseTvChannelDataAsync(tvchannel, newName, isPublished);

            //localization
            await CopyLocalizationDataAsync(tvchannel, tvchannelCopy);

            //copy tvchannel tags
            foreach (var tvchannelTag in await _tvchannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvchannel.Id)) 
                await _tvchannelTagService.InsertTvChannelTvChannelTagMappingAsync(new TvChannelTvChannelTagMapping { TvChannelTagId = tvchannelTag.Id, TvChannelId = tvchannelCopy.Id });

            await _tvchannelService.UpdateTvChannelAsync(tvchannelCopy);

            //copy tvchannel pictures
            var originalNewPictureIdentifiers = await CopyTvChannelPicturesAsync(tvchannel, newName, copyMultimedia, tvchannelCopy);

            //copy tvchannel videos
            await CopyTvChannelVideosAsync(tvchannel, copyMultimedia, tvchannelCopy);

            //quantity change history
            await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannelCopy, tvchannel.StockQuantity, tvchannel.StockQuantity, tvchannel.WarehouseId,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CopyTvChannel"), tvchannel.Id));

            //tvchannel specifications
            await CopyTvChannelSpecificationsAsync(tvchannel, tvchannelCopy);

            //tvchannel <-> warehouses mappings
            await CopyWarehousesMappingAsync(tvchannel, tvchannelCopy);
            //tvchannel <-> categories mappings
            await CopyCategoriesMappingAsync(tvchannel, tvchannelCopy);
            //tvchannel <-> manufacturers mappings
            await CopyManufacturersMappingAsync(tvchannel, tvchannelCopy);
            //tvchannel <-> related tvchannels mappings
            await CopyRelatedTvChannelsMappingAsync(tvchannel, tvchannelCopy);
            //tvchannel <-> cross sells mappings
            await CopyCrossSellsMappingAsync(tvchannel, tvchannelCopy);
            //tvchannel <-> attributes mappings
            await CopyAttributesMappingAsync(tvchannel, tvchannelCopy, originalNewPictureIdentifiers);
            //tvchannel <-> discounts mapping
            await CopyDiscountsMappingAsync(tvchannel, tvchannelCopy);

            //store mapping
            var selectedStoreIds = await _storeMappingService.GetStoresIdsWithAccessAsync(tvchannel);
            foreach (var id in selectedStoreIds) 
                await _storeMappingService.InsertStoreMappingAsync(tvchannelCopy, id);

            //user role mapping
            var userRoleIds = await _aclService.GetUserRoleIdsWithAccessAsync(tvchannel);
            foreach (var id in userRoleIds)
                await _aclService.InsertAclRecordAsync(tvchannelCopy, id);

            //tier prices
            await CopyTierPricesAsync(tvchannel, tvchannelCopy);

            //update "HasTierPrices" and "HasDiscountsApplied" properties
            await _tvchannelService.UpdateHasTierPricesPropertyAsync(tvchannelCopy);
            await _tvchannelService.UpdateHasDiscountsAppliedAsync(tvchannelCopy);

            //associated tvchannels
            await CopyAssociatedTvChannelsAsync(tvchannel, isPublished, copyMultimedia, copyAssociatedTvChannels, tvchannelCopy);

            return tvchannelCopy;
        }

        #endregion
    }
}