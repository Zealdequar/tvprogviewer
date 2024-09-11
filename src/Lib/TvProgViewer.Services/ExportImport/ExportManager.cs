using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ClosedXML.Excel;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.ExportImport.Help;
using TvProgViewer.Services.Forums;
using TvProgViewer.Services.Gdpr;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Tax;
using TvProgViewer.Services.Vendors;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TvProgViewer.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IAddressService _addressService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserAttributeFormatter _userAttributeFormatter;
        private readonly IUserService _userService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IForumService _forumService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;
        private readonly ITvChannelTagService _tvChannelTagService;
        private readonly ITvChannelTemplateService _tvChannelTemplateService;
        private readonly IShipmentService _shipmentService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly TvChannelEditorSettings _tvChannelEditorSettings;

        #endregion

        #region Ctor

        public ExportManager(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            IUserActivityService userActivityService,
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            ForumSettings forumSettings,
            IAddressService addressService,
            ICategoryService categoryService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IUserAttributeFormatter userAttributeFormatter,
            IUserService userService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IForumService forumService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService,
            ITvChannelTagService tvChannelTagService,
            ITvChannelTemplateService tvChannelTemplateService,
            IShipmentService shipmentService,
            ISpecificationAttributeService specificationAttributeService,
            IStateProvinceService stateProvinceService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            TvChannelEditorSettings tvChannelEditorSettings)
        {
            _addressSettings = addressSettings;
            _catalogSettings = catalogSettings;
            _userActivityService = userActivityService;
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _addressService = addressService;
            _forumSettings = forumSettings;
            _categoryService = categoryService;
            _countryService = countryService;
            _currencyService = currencyService;
            _userAttributeFormatter = userAttributeFormatter;
            _userService = userService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _discountService = discountService;
            _forumService = forumService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _manufacturerService = manufacturerService;
            _measureService = measureService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
            _tvChannelTagService = tvChannelTagService;
            _tvChannelTemplateService = tvChannelTemplateService;
            _shipmentService = shipmentService;
            _specificationAttributeService = specificationAttributeService;
            _stateProvinceService = stateProvinceService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _taxCategoryService = taxCategoryService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _tvChannelEditorSettings = tvChannelEditorSettings;
        }

        #endregion

        #region Utilities

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<int> WriteCategoriesAsync(XmlWriter xmlWriter, int parentCategoryId, int totalCategories)
        {
            var categories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(parentCategoryId, true);
            if (categories == null || !categories.Any())
                return totalCategories;

            totalCategories += categories.Count;

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            foreach (var category in categories)
            {
                await xmlWriter.WriteStartElementAsync("Category");

                await xmlWriter.WriteStringAsync("Id", category.Id);

                await WriteLocalizedPropertyXmlAsync(category, c => c.Name, xmlWriter, languages);
                await WriteLocalizedPropertyXmlAsync(category, c => c.Description, xmlWriter, languages);
                await xmlWriter.WriteStringAsync("CategoryTemplateId", category.CategoryTemplateId);
                await WriteLocalizedPropertyXmlAsync(category, c => c.MetaKeywords, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
                await WriteLocalizedPropertyXmlAsync(category, c => c.MetaDescription, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
                await WriteLocalizedPropertyXmlAsync(category, c => c.MetaTitle, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
                await WriteLocalizedSeNameXmlAsync(category, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("ParentCategoryId", category.ParentCategoryId);
                await xmlWriter.WriteStringAsync("PictureId", category.PictureId);
                await xmlWriter.WriteStringAsync("PageSize", category.PageSize, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("AllowUsersToSelectPageSize", category.AllowUsersToSelectPageSize, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("PageSizeOptions", category.PageSizeOptions, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceRangeFiltering", category.PriceRangeFiltering, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceFrom", category.PriceFrom, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceTo", category.PriceTo, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("ManuallyPriceRange", category.ManuallyPriceRange, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("ShowOnHomepage", category.ShowOnHomepage, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("IncludeInTopMenu", category.IncludeInTopMenu, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("Published", category.Published, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("Deleted", category.Deleted, true);
                await xmlWriter.WriteStringAsync("DisplayOrder", category.DisplayOrder);
                await xmlWriter.WriteStringAsync("CreatedOnUtc", category.CreatedOnUtc, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("UpdatedOnUtc", category.UpdatedOnUtc, await IgnoreExportCategoryPropertyAsync());

                await xmlWriter.WriteStartElementAsync("TvChannels");
                var tvChannelCategories = await _categoryService.GetTvChannelCategoriesByCategoryIdAsync(category.Id, showHidden: true);
                foreach (var tvChannelCategory in tvChannelCategories)
                {
                    var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelCategory.TvChannelId);
                    if (tvChannel == null || tvChannel.Deleted)
                        continue;

                    await xmlWriter.WriteStartElementAsync("TvChannelCategory");
                    await xmlWriter.WriteStringAsync("TvChannelCategoryId", tvChannelCategory.Id);
                    await xmlWriter.WriteStringAsync("TvChannelId", tvChannelCategory.TvChannelId);
                    await WriteLocalizedPropertyXmlAsync(tvChannel, p => p.Name, xmlWriter, languages, overriddenNodeName: "TvChannelName");
                    await xmlWriter.WriteStringAsync("IsFeaturedTvChannel", tvChannelCategory.IsFeaturedTvChannel);
                    await xmlWriter.WriteStringAsync("DisplayOrder", tvChannelCategory.DisplayOrder);
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();

                await xmlWriter.WriteStartElementAsync("SubCategories");
                totalCategories = await WriteCategoriesAsync(xmlWriter, category.Id, totalCategories);
                await xmlWriter.WriteEndElementAsync();
                await xmlWriter.WriteEndElementAsync();
            }

            return totalCategories;
        }

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the path to the image file
        /// </returns>
        protected virtual async Task<string> GetPicturesAsync(int pictureId)
        {
            var picture = await _pictureService.GetPictureByIdAsync(pictureId);

            return await _pictureService.GetThumbLocalPathAsync(picture);
        }

        /// <summary>
        /// Returns the list of categories for a tvChannel separated by a ";"
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of categories
        /// </returns>
        protected virtual async Task<object> GetCategoriesAsync(TvChannel tvChannel)
        {
            string categoryNames = null;
            foreach (var pc in await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvChannel.Id, true))
            {
                if (_catalogSettings.ExportImportRelatedEntitiesByName)
                {
                    var category = await _categoryService.GetCategoryByIdAsync(pc.CategoryId);
                    categoryNames += _catalogSettings.ExportImportTvChannelCategoryBreadcrumb
                        ? await _categoryService.GetFormattedBreadCrumbAsync(category)
                        : category.Name;
                }
                else
                {
                    categoryNames += pc.CategoryId.ToString();
                }

                categoryNames += ";";
            }

            return categoryNames;
        }

        /// <summary>
        /// Returns the list of manufacturer for a tvChannel separated by a ";"
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of manufacturer
        /// </returns>
        protected virtual async Task<object> GetManufacturersAsync(TvChannel tvChannel)
        {
            string manufacturerNames = null;
            foreach (var pm in await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvChannel.Id, true))
            {
                if (_catalogSettings.ExportImportRelatedEntitiesByName)
                {
                    var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
                    manufacturerNames += manufacturer.Name;
                }
                else
                {
                    manufacturerNames += pm.ManufacturerId.ToString();
                }

                manufacturerNames += ";";
            }

            return manufacturerNames;
        }

        /// <summary>
        /// Returns the list of limited to stores for a tvChannel separated by a ";"
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of store
        /// </returns>
        protected virtual async Task<object> GetLimitedToStoresAsync(TvChannel tvChannel)
        {
            string limitedToStores = null;
            foreach (var storeMapping in await _storeMappingService.GetStoreMappingsAsync(tvChannel))
            {
                var store = await _storeService.GetStoreByIdAsync(storeMapping.StoreId);

                limitedToStores += _catalogSettings.ExportImportRelatedEntitiesByName ? store.Name : store.Id.ToString();

                limitedToStores += ";";
            }

            return limitedToStores;
        }

        /// <summary>
        /// Returns the list of tvChannel tag for a tvChannel separated by a ";"
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvChannel tag
        /// </returns>
        protected virtual async Task<object> GetTvChannelTagsAsync(TvChannel tvChannel)
        {
            string tvChannelTagNames = null;

            var tvChannelTags = await _tvChannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvChannel.Id);

            if (!tvChannelTags?.Any() ?? true)
                return null;

            foreach (var tvChannelTag in tvChannelTags)
            {
                tvChannelTagNames += _catalogSettings.ExportImportRelatedEntitiesByName
                    ? tvChannelTag.Name
                    : tvChannelTag.Id.ToString();

                tvChannelTagNames += ";";
            }

            return tvChannelTagNames;
        }

        /// <summary>
        /// Returns the image at specified index associated with the tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="pictureIndex">Picture index to get</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the image thumb local path
        /// </returns>
        protected virtual async Task<string> GetPictureAsync(TvChannel tvChannel, short pictureIndex)
        {
            // we need only the picture at a specific index, no need to get more pictures than that
            var recordsToReturn = pictureIndex + 1;
            var pictures = await _pictureService.GetPicturesByTvChannelIdAsync(tvChannel.Id, recordsToReturn);

            return pictures.Count > pictureIndex ? await _pictureService.GetThumbLocalPathAsync(pictures[pictureIndex]) : null;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<bool> IgnoreExportTvChannelPropertyAsync(Func<TvChannelEditorSettings, bool> func)
        {
            var tvChannelAdvancedMode = true;
            try
            {
                tvChannelAdvancedMode = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), "tvChannel-advanced-mode");
            }
            catch (ArgumentNullException)
            {
            }

            return !tvChannelAdvancedMode && !func(_tvChannelEditorSettings);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<bool> IgnoreExportCategoryPropertyAsync()
        {
            try
            {
                return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), "category-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<bool> IgnoreExportManufacturerPropertyAsync()
        {
            try
            {
                return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), "manufacturer-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<bool> IgnoreExportLimitedToStoreAsync()
        {
            return _catalogSettings.IgnoreStoreLimitations ||
                   !_catalogSettings.ExportImportTvChannelUseLimitedToStores ||
                   (await _storeService.GetAllStoresAsync()).Count == 1;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<TProperty> GetLocalizedAsync<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> keySelector,
            Language language) where TEntity : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                return default(TProperty);

            return await _localizationService.GetLocalizedAsync(entity, keySelector, language.Id, false);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<PropertyManager<ExportTvChannelAttribute, Language>> GetTvChannelAttributeManagerAsync(IList<Language> languages)
        {
            var attributeProperties = new[]
            {
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeId", (p, l) => p.AttributeId),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeName", (p, l) => p.AttributeName),
                new PropertyByName<ExportTvChannelAttribute, Language>("DefaultValue", (p, l) => p.DefaultValue),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValidationMinLength", (p, l) => p.ValidationMinLength),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValidationMaxLength", (p, l) => p.ValidationMaxLength),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValidationFileAllowedExtensions", (p, l) => p.ValidationFileAllowedExtensions),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValidationFileMaximumSize", (p, l) => p.ValidationFileMaximumSize),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeTextPrompt", (p, l) => p.AttributeTextPrompt),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeIsRequired", (p, l) => p.AttributeIsRequired),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeControlType", (p, l) => p.AttributeControlTypeId)
                {
                    DropDownElements = await AttributeControlType.TextBox.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeDisplayOrder", (p, l) => p.AttributeDisplayOrder),
                new PropertyByName<ExportTvChannelAttribute, Language>("TvChannelAttributeValueId", (p, l) => p.Id),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValueName", (p, l) => p.Name),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeValueType", (p, l) => p.AttributeValueTypeId)
                {
                    DropDownElements = await AttributeValueType.Simple.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<ExportTvChannelAttribute, Language>("AssociatedTvChannelId", (p, l) => p.AssociatedTvChannelId),
                new PropertyByName<ExportTvChannelAttribute, Language>("ColorSquaresRgb", (p, l) => p.ColorSquaresRgb),
                new PropertyByName<ExportTvChannelAttribute, Language>("ImageSquaresPictureId", (p, l) => p.ImageSquaresPictureId),
                new PropertyByName<ExportTvChannelAttribute, Language>("PriceAdjustment", (p, l) => p.PriceAdjustment),
                new PropertyByName<ExportTvChannelAttribute, Language>("PriceAdjustmentUsePercentage", (p, l) => p.PriceAdjustmentUsePercentage),
                new PropertyByName<ExportTvChannelAttribute, Language>("WeightAdjustment", (p, l) => p.WeightAdjustment),
                new PropertyByName<ExportTvChannelAttribute, Language>("Cost", (p, l) => p.Cost),
                new PropertyByName<ExportTvChannelAttribute, Language>("UserEntersQty", (p, l) => p.UserEntersQty),
                new PropertyByName<ExportTvChannelAttribute, Language>("Quantity", (p, l) => p.Quantity),
                new PropertyByName<ExportTvChannelAttribute, Language>("IsPreSelected", (p, l) => p.IsPreSelected),
                new PropertyByName<ExportTvChannelAttribute, Language>("DisplayOrder", (p, l) => p.DisplayOrder),
                new PropertyByName<ExportTvChannelAttribute, Language>("PictureId", (p, l) => p.PictureId)
            };

            var localizedProperties = new[]
            {
                new PropertyByName<ExportTvChannelAttribute, Language>("DefaultValue", async (p, l) =>
                    await GetLocalizedAsync(await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(p.AttributeMappingId), x => x.DefaultValue, l)),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeTextPrompt", async (p, l) =>
                    await GetLocalizedAsync(await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(p.AttributeMappingId), x => x.TextPrompt, l)),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValueName", async (p, l) =>
                    await GetLocalizedAsync(await _tvChannelAttributeService.GetTvChannelAttributeValueByIdAsync(p.Id), x => x.Name, l)),
            };

            return new PropertyManager<ExportTvChannelAttribute, Language>(attributeProperties, _catalogSettings, localizedProperties, languages);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<PropertyManager<ExportSpecificationAttribute, Language>> GetSpecificationAttributeManagerAsync(IList<Language> languages)
        {
            var attributeProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute, Language>("AttributeType", (p, l) => p.AttributeTypeId)
                {
                    DropDownElements = await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<ExportSpecificationAttribute, Language>("SpecificationAttribute", (p, l) => p.SpecificationAttributeId)
                {
                    DropDownElements = (await _specificationAttributeService.GetSpecificationAttributesAsync()).Select(sa => sa as BaseEntity).ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty)
                },
                new PropertyByName<ExportSpecificationAttribute, Language>("CustomValue", (p, l) => p.CustomValue),
                new PropertyByName<ExportSpecificationAttribute, Language>("SpecificationAttributeOptionId", (p, l) => p.SpecificationAttributeOptionId),
                new PropertyByName<ExportSpecificationAttribute, Language>("AllowFiltering", (p, l) => p.AllowFiltering),
                new PropertyByName<ExportSpecificationAttribute, Language>("ShowOnTvChannelPage", (p, l) => p.ShowOnTvChannelPage),
                new PropertyByName<ExportSpecificationAttribute, Language>("DisplayOrder", (p, l) => p.DisplayOrder)
            };

            var localizedProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute, Language>("CustomValue", async (p, l) =>
                    await GetLocalizedAsync(await _specificationAttributeService.GetTvChannelSpecificationAttributeByIdAsync(p.Id), x => x.CustomValue, l)),
            };

            return new PropertyManager<ExportSpecificationAttribute, Language>(attributeProperties, _catalogSettings, localizedProperties, languages);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<byte[]> ExportTvChannelsToXlsxWithAttributesAsync(PropertyByName<TvChannel, Language>[] properties, PropertyByName<TvChannel, Language>[] localizedProperties, IEnumerable<TvChannel> itemsToExport, IList<Language> languages)
        {
            var tvChannelAttributeManager = await GetTvChannelAttributeManagerAsync(languages);
            var specificationAttributeManager = await GetSpecificationAttributeManagerAsync(languages);

            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                // Worksheet names cannot be more than 31 characters
                var worksheet = workbook.Worksheets.Add(typeof(TvChannel).Name);
                var fpWorksheet = workbook.Worksheets.Add("TvChannelsFilters");
                fpWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;
                var fbaWorksheet = workbook.Worksheets.Add("TvChannelAttributesFilters");
                fbaWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;
                var fsaWorksheet = workbook.Worksheets.Add("SpecificationAttributesFilters");
                fsaWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

                //create Headers and format them 
                var manager = new PropertyManager<TvChannel, Language>(properties, _catalogSettings, localizedProperties, languages);
                manager.WriteDefaultCaption(worksheet);

                var localizedWorksheets = new List<(Language Language, IXLWorksheet Worksheet)>();
                if (languages.Count >= 2)
                {
                    foreach (var language in languages)
                    {
                        var lws = workbook.Worksheets.Add(language.UniqueSeoCode);
                        localizedWorksheets.Add(new(language, lws));
                        manager.WriteLocalizedCaption(lws);
                    }
                }

                var row = 2;
                foreach (var item in itemsToExport)
                {
                    manager.CurrentObject = item;
                    await manager.WriteDefaultToXlsxAsync(worksheet, row, fWorksheet: fpWorksheet);

                    foreach (var lws in localizedWorksheets)
                    {
                        manager.CurrentLanguage = lws.Language;
                        await manager.WriteLocalizedToXlsxAsync(lws.Worksheet, row, fWorksheet: fpWorksheet);
                    }
                    row++;

                    if (_catalogSettings.ExportImportTvChannelAttributes)
                        row = await ExportTvChannelAttributesAsync(item, tvChannelAttributeManager, worksheet, localizedWorksheets, row, fbaWorksheet);

                    if (_catalogSettings.ExportImportTvChannelSpecificationAttributes)
                        row = await ExportSpecificationAttributesAsync(item, specificationAttributeManager, worksheet, localizedWorksheets, row, fsaWorksheet);
                }

                workbook.SaveAs(stream);
            }

            return stream.ToArray();
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<int> ExportTvChannelAttributesAsync(TvChannel item, PropertyManager<ExportTvChannelAttribute, Language> attributeManager,
            IXLWorksheet worksheet, IList<(Language Language, IXLWorksheet Worksheet)> localizedWorksheets, int row, IXLWorksheet faWorksheet)
        {
            var attributes = await (await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(item.Id))
                .SelectManyAwait(async pam =>
                {
                    var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(pam.TvChannelAttributeId);

                    var values = await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(pam.Id);

                    if (values?.Any() ?? false)
                        return values.Select(pav =>
                            new ExportTvChannelAttribute
                            {
                                AttributeId = tvChannelAttribute.Id,
                                AttributeName = tvChannelAttribute.Name,
                                AttributeTextPrompt = pam.TextPrompt,
                                AttributeIsRequired = pam.IsRequired,
                                AttributeControlTypeId = pam.AttributeControlTypeId,
                                AttributeMappingId = pav.TvChannelAttributeMappingId,
                                AssociatedTvChannelId = pav.AssociatedTvChannelId,
                                AttributeDisplayOrder = pam.DisplayOrder,
                                Id = pav.Id,
                                Name = pav.Name,
                                AttributeValueTypeId = pav.AttributeValueTypeId,
                                ColorSquaresRgb = pav.ColorSquaresRgb,
                                ImageSquaresPictureId = pav.ImageSquaresPictureId,
                                PriceAdjustment = pav.PriceAdjustment,
                                PriceAdjustmentUsePercentage = pav.PriceAdjustmentUsePercentage,
                                WeightAdjustment = pav.WeightAdjustment,
                                Cost = pav.Cost,
                                UserEntersQty = pav.UserEntersQty,
                                Quantity = pav.Quantity,
                                IsPreSelected = pav.IsPreSelected,
                                DisplayOrder = pav.DisplayOrder,
                                PictureId = pav.PictureId
                            });

                    var attribute = new ExportTvChannelAttribute
                    {
                        AttributeId = tvChannelAttribute.Id,
                        AttributeName = tvChannelAttribute.Name,
                        AttributeTextPrompt = pam.TextPrompt,
                        AttributeIsRequired = pam.IsRequired,
                        AttributeControlTypeId = pam.AttributeControlTypeId,
                    };

                    //validation rules
                    if (!pam.ValidationRulesAllowed())
                        return new List<ExportTvChannelAttribute> { attribute };

                    attribute.ValidationMinLength = pam.ValidationMinLength;
                    attribute.ValidationMaxLength = pam.ValidationMaxLength;
                    attribute.ValidationFileAllowedExtensions = pam.ValidationFileAllowedExtensions;
                    attribute.ValidationFileMaximumSize = pam.ValidationFileMaximumSize;
                    attribute.DefaultValue = pam.DefaultValue;

                    return new List<ExportTvChannelAttribute>
                    {
                        attribute
                    };
                }).ToListAsync();

            if (!attributes.Any())
                return row;

            attributeManager.WriteDefaultCaption(worksheet, row, ExportTvChannelAttribute.TvChannelAttributeCellOffset);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapse();

            foreach (var lws in localizedWorksheets)
            {
                attributeManager.WriteLocalizedCaption(lws.Worksheet, row, ExportTvChannelAttribute.TvChannelAttributeCellOffset);
                lws.Worksheet.Row(row).OutlineLevel = 1;
                lws.Worksheet.Row(row).Collapse();
            }

            foreach (var exportTvChannelAttribute in attributes)
            {
                row++;
                attributeManager.CurrentObject = exportTvChannelAttribute;
                await attributeManager.WriteDefaultToXlsxAsync(worksheet, row, ExportTvChannelAttribute.TvChannelAttributeCellOffset, faWorksheet);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapse();

                foreach (var lws in localizedWorksheets)
                {
                    attributeManager.CurrentLanguage = lws.Language;
                    await attributeManager.WriteLocalizedToXlsxAsync(lws.Worksheet, row, ExportTvChannelAttribute.TvChannelAttributeCellOffset, faWorksheet);
                    lws.Worksheet.Row(row).OutlineLevel = 1;
                    lws.Worksheet.Row(row).Collapse();
                }
            }

            return row + 1;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<int> ExportSpecificationAttributesAsync(TvChannel item, PropertyManager<ExportSpecificationAttribute, Language> attributeManager,
            IXLWorksheet worksheet, IList<(Language Language, IXLWorksheet Worksheet)> localizedWorksheets, int row, IXLWorksheet faWorksheet)
        {
            var attributes = await (await _specificationAttributeService
                .GetTvChannelSpecificationAttributesAsync(item.Id)).SelectAwait(
                async psa => await ExportSpecificationAttribute.CreateAsync(psa, _specificationAttributeService)).ToListAsync();

            if (!attributes.Any())
                return row;

            attributeManager.WriteDefaultCaption(worksheet, row, ExportTvChannelAttribute.TvChannelAttributeCellOffset);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapse();

            foreach (var lws in localizedWorksheets)
            {
                attributeManager.WriteLocalizedCaption(lws.Worksheet, row, ExportTvChannelAttribute.TvChannelAttributeCellOffset);
                lws.Worksheet.Row(row).OutlineLevel = 1;
                lws.Worksheet.Row(row).Collapse();
            }

            foreach (var exportTvChannelAttribute in attributes)
            {
                row++;
                attributeManager.CurrentObject = exportTvChannelAttribute;
                await attributeManager.WriteDefaultToXlsxAsync(worksheet, row, ExportTvChannelAttribute.TvChannelAttributeCellOffset, faWorksheet);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapse();

                foreach (var lws in localizedWorksheets)
                {
                    attributeManager.CurrentLanguage = lws.Language;
                    await attributeManager.WriteLocalizedToXlsxAsync(lws.Worksheet, row, ExportTvChannelAttribute.TvChannelAttributeCellOffset, faWorksheet);
                    lws.Worksheet.Row(row).OutlineLevel = 1;
                    lws.Worksheet.Row(row).Collapse();
                }
            }

            return row + 1;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<byte[]> ExportOrderToXlsxWithTvChannelsAsync(PropertyByName<Order, Language>[] properties, IEnumerable<Order> itemsToExport)
        {
            var orderItemProperties = new[]
            {
                new PropertyByName<OrderItem, Language>("OrderItemGuid", (oi, l) => oi.OrderItemGuid),
                new PropertyByName<OrderItem, Language>("Name", async (oi, l) => (await _tvChannelService.GetTvChannelByIdAsync(oi.TvChannelId)).Name),
                new PropertyByName<OrderItem, Language>("Sku", async (oi, l) => await _tvChannelService.FormatSkuAsync(await _tvChannelService.GetTvChannelByIdAsync(oi.TvChannelId), oi.AttributesXml)),
                new PropertyByName<OrderItem, Language>("PriceExclTax", (oi, l) => oi.UnitPriceExclTax),
                new PropertyByName<OrderItem, Language>("PriceInclTax", (oi, l) => oi.UnitPriceInclTax),
                new PropertyByName<OrderItem, Language>("Quantity", (oi, l) => oi.Quantity),
                new PropertyByName<OrderItem, Language>("DiscountExclTax", (oi, l) => oi.DiscountAmountExclTax),
                new PropertyByName<OrderItem, Language>("DiscountInclTax", (oi, l) => oi.DiscountAmountInclTax),
                new PropertyByName<OrderItem, Language>("TotalExclTax", (oi, l) => oi.PriceExclTax),
                new PropertyByName<OrderItem, Language>("TotalInclTax", (oi, l) => oi.PriceInclTax)
            };

            var orderItemsManager = new PropertyManager<OrderItem, Language>(orderItemProperties, _catalogSettings);

            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                // Worksheet names cannot be more than 31 characters
                var worksheet = workbook.Worksheets.Add(typeof(Order).Name);
                var fpWorksheet = workbook.Worksheets.Add("DataForTvChannelsFilters");
                fpWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

                //create Headers and format them 
                var manager = new PropertyManager<Order, Language>(properties, _catalogSettings);
                manager.WriteDefaultCaption(worksheet);

                var row = 2;
                foreach (var order in itemsToExport)
                {
                    manager.CurrentObject = order;
                    await manager.WriteDefaultToXlsxAsync(worksheet, row++);

                    //a vendor should have access only to his tvChannels
                    var vendor = await _workContext.GetCurrentVendorAsync();
                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

                    if (!orderItems.Any())
                        continue;

                    orderItemsManager.WriteDefaultCaption(worksheet, row, 2);
                    worksheet.Row(row).OutlineLevel = 1;
                    worksheet.Row(row).Collapse();

                    foreach (var orderItem in orderItems)
                    {
                        row++;
                        orderItemsManager.CurrentObject = orderItem;
                        await orderItemsManager.WriteDefaultToXlsxAsync(worksheet, row, 2, fpWorksheet);
                        worksheet.Row(row).OutlineLevel = 1;
                        worksheet.Row(row).Collapse();
                    }

                    row++;
                }

                workbook.SaveAs(stream);
            }

            return stream.ToArray();
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<object> GetCustomUserAttributesAsync(User user)
        {
            return await _userAttributeFormatter.FormatAttributesAsync(user.CustomUserAttributesXML, ";");
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task WriteLocalizedPropertyXmlAsync<TEntity, TPropType>(TEntity entity, Expression<Func<TEntity, TPropType>> keySelector,
            XmlWriter xmlWriter, IList<Language> languages, bool ignore = false, string overriddenNodeName = null)
            where TEntity : BaseEntity, ILocalizedEntity
        {
            if (ignore)
                return;

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var localeKeyGroup = entity.GetType().Name;
            var localeKey = propInfo.Name;

            var nodeName = localeKey;
            if (!string.IsNullOrWhiteSpace(overriddenNodeName))
                nodeName = overriddenNodeName;

            await xmlWriter.WriteStartElementAsync(nodeName);
            await xmlWriter.WriteStringAsync("Standard", propInfo.GetValue(entity));

            if (languages.Count >= 2)
            {
                await xmlWriter.WriteStartElementAsync("Locales");

                var properties = await _localizedEntityService.GetEntityLocalizedPropertiesAsync(entity.Id, localeKeyGroup, localeKey);
                foreach (var language in languages)
                    if (properties.FirstOrDefault(lp => lp.LanguageId == language.Id) is LocalizedProperty localizedProperty)
                        await xmlWriter.WriteStringAsync(language.UniqueSeoCode, localizedProperty.LocaleValue);

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task WriteLocalizedSeNameXmlAsync<TEntity>(TEntity entity, XmlWriter xmlWriter, IList<Language> languages,
            bool ignore = false, string overriddenNodeName = null)
            where TEntity : BaseEntity, ISlugSupported
        {
            if (ignore)
                return;

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var nodeName = "SEName";
            if (!string.IsNullOrWhiteSpace(overriddenNodeName))
                nodeName = overriddenNodeName;

            await xmlWriter.WriteStartElementAsync(nodeName);
            await xmlWriter.WriteStringAsync("Standard", await _urlRecordService.GetSeNameAsync(entity, 0));

            if (languages.Count >= 2)
            {
                await xmlWriter.WriteStartElementAsync("Locales");

                foreach (var language in languages)
                    if (await _urlRecordService.GetSeNameAsync(entity, language.Id, returnDefaultValue: false) is string seName && !string.IsNullOrWhiteSpace(seName))
                        await xmlWriter.WriteStringAsync(language.UniqueSeoCode, seName);

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Export manufacturer list to XML
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportManufacturersToXmlAsync(IList<Manufacturer> manufacturers)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Manufacturers");
            await xmlWriter.WriteAttributeStringAsync("Version", TvProgVersion.CURRENT_VERSION);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            foreach (var manufacturer in manufacturers)
            {
                await xmlWriter.WriteStartElementAsync("Manufacturer");

                await xmlWriter.WriteStringAsync("ManufacturerId", manufacturer.Id.ToString());
                await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.Name, xmlWriter, languages);
                await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.Description, xmlWriter, languages);
                await xmlWriter.WriteStringAsync("ManufacturerTemplateId", manufacturer.ManufacturerTemplateId);
                await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.MetaKeywords, xmlWriter, languages, await IgnoreExportManufacturerPropertyAsync());
                await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.MetaDescription, xmlWriter, languages, await IgnoreExportManufacturerPropertyAsync());
                await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.MetaTitle, xmlWriter, languages, await IgnoreExportManufacturerPropertyAsync());
                await WriteLocalizedSeNameXmlAsync(manufacturer, xmlWriter, languages, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("PictureId", manufacturer.PictureId);
                await xmlWriter.WriteStringAsync("PageSize", manufacturer.PageSize, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("AllowUsersToSelectPageSize", manufacturer.AllowUsersToSelectPageSize, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("PageSizeOptions", manufacturer.PageSizeOptions, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceRangeFiltering", manufacturer.PriceRangeFiltering, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceFrom", manufacturer.PriceFrom, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("PriceTo", manufacturer.PriceTo, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("ManuallyPriceRange", manufacturer.ManuallyPriceRange, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("Published", manufacturer.Published, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("Deleted", manufacturer.Deleted, true);
                await xmlWriter.WriteStringAsync("DisplayOrder", manufacturer.DisplayOrder);
                await xmlWriter.WriteStringAsync("CreatedOnUtc", manufacturer.CreatedOnUtc, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("UpdatedOnUtc", manufacturer.UpdatedOnUtc, await IgnoreExportManufacturerPropertyAsync());

                await xmlWriter.WriteStartElementAsync("TvChannels");
                var tvChannelManufacturers = await _manufacturerService.GetTvChannelManufacturersByManufacturerIdAsync(manufacturer.Id, showHidden: true);
                if (tvChannelManufacturers != null)
                {
                    foreach (var tvChannelManufacturer in tvChannelManufacturers)
                    {
                        var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelManufacturer.TvChannelId);
                        if (tvChannel == null || tvChannel.Deleted)
                            continue;

                        await xmlWriter.WriteStartElementAsync("TvChannelManufacturer");
                        await xmlWriter.WriteStringAsync("TvChannelManufacturerId", tvChannelManufacturer.Id);
                        await xmlWriter.WriteStringAsync("TvChannelId", tvChannelManufacturer.TvChannelId);
                        await WriteLocalizedPropertyXmlAsync(tvChannel, p => p.Name, xmlWriter, languages, overriddenNodeName: "TvChannelName");
                        await xmlWriter.WriteStringAsync("IsFeaturedTvChannel", tvChannelManufacturer.IsFeaturedTvChannel);
                        await xmlWriter.WriteStringAsync("DisplayOrder", tvChannelManufacturer.DisplayOrder);
                        await xmlWriter.WriteEndElementAsync();
                    }
                }

                await xmlWriter.WriteEndElementAsync();
                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            //activity log
            await _userActivityService.InsertActivityAsync("ExportManufacturers",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportManufacturers"), manufacturers.Count));

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export manufacturers to XLSX
        /// </summary>
        /// <param name="manufacturers">Manufactures</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<byte[]> ExportManufacturersToXlsxAsync(IEnumerable<Manufacturer> manufacturers)
        {
            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            var localizedProperties = new[]
            {
                new PropertyByName<Manufacturer, Language>("Id", (p, l) => p.Id),
                new PropertyByName<Manufacturer, Language>("Name", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.Name, l.Id, false)),
                new PropertyByName<Manufacturer, Language>("MetaKeywords", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaKeywords, l.Id, false)),
                new PropertyByName<Manufacturer, Language>("MetaDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaDescription, l.Id, false)),
                new PropertyByName<Manufacturer, Language>("MetaTitle", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaTitle, l.Id, false)),
                new PropertyByName<Manufacturer, Language>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, l.Id, returnDefaultValue: false), await IgnoreExportManufacturerPropertyAsync())
            };

            //property manager 
            var manager = new PropertyManager<Manufacturer, Language>(new[]
            {
                new PropertyByName<Manufacturer, Language>("Id", (p, l) => p.Id),
                new PropertyByName<Manufacturer, Language>("Name", (p, l) => p.Name),
                new PropertyByName<Manufacturer, Language>("Description", (p, l) => p.Description),
                new PropertyByName<Manufacturer, Language>("ManufacturerTemplateId", (p, l) => p.ManufacturerTemplateId),
                new PropertyByName<Manufacturer, Language>("MetaKeywords", (p, l)=> p.MetaKeywords, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("MetaDescription", (p, l) => p.MetaDescription, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("MetaTitle", (p, l) => p.MetaTitle, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("Picture", async (p, l) => await GetPicturesAsync(p.PictureId)),
                new PropertyByName<Manufacturer, Language>("PageSize", (p, l) => p.PageSize, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("AllowUsersToSelectPageSize", (p, l) => p.AllowUsersToSelectPageSize, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("PageSizeOptions", (p, l) => p.PageSizeOptions, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("PriceRangeFiltering", (p, l) => p.PriceRangeFiltering, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("PriceFrom", (p, l) => p.PriceFrom, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("PriceTo", (p, l) => p.PriceTo, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("ManuallyPriceRange", (p, l) => p.ManuallyPriceRange, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("Published", (p, l) => p.Published, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer, Language>("DisplayOrder", (p, l) => p.DisplayOrder)
            }, _catalogSettings, localizedProperties, languages);

            //activity log
            await _userActivityService.InsertActivityAsync("ExportManufacturers",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportManufacturers"), manufacturers.Count()));

            return await manager.ExportToXlsxAsync(manufacturers);
        }

        /// <summary>
        /// Export category list to XML
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportCategoriesToXmlAsync()
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Categories");
            await xmlWriter.WriteAttributeStringAsync("Version", TvProgVersion.CURRENT_VERSION);
            var totalCategories = await WriteCategoriesAsync(xmlWriter, 0, 0);
            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            //activity log
            await _userActivityService.InsertActivityAsync("ExportCategories",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCategories"), totalCategories));

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<byte[]> ExportCategoriesToXlsxAsync(IList<Category> categories)
        {
            var parentCategories = new List<Category>();
            if (_catalogSettings.ExportImportCategoriesUsingCategoryName)
                //performance optimization, load all parent categories in one SQL request
                parentCategories.AddRange(await _categoryService.GetCategoriesByIdsAsync(categories.Select(c => c.ParentCategoryId).Where(id => id != 0).ToArray()));

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            var localizedProperties = new[]
            {
                new PropertyByName<Category, Language>("Id", (p, l) => p.Id),
                new PropertyByName<Category, Language>("Name", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.Name, l.Id, false)),
                new PropertyByName<Category, Language>("MetaKeywords", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaKeywords, l.Id, false)),
                new PropertyByName<Category, Language>("MetaDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaDescription, l.Id, false)),
                new PropertyByName<Category, Language>("MetaTitle", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaTitle, l.Id, false)),
                new PropertyByName<Category, Language>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, l.Id, returnDefaultValue: false), await IgnoreExportCategoryPropertyAsync())
            };

            //property manager 
            var manager = new PropertyManager<Category, Language>(new[]
            {
                new PropertyByName<Category, Language>("Id", (p, l) => p.Id),
                new PropertyByName<Category, Language>("Name", (p, l) => p.Name),
                new PropertyByName<Category, Language>("Description", (p, l) => p.Description),
                new PropertyByName<Category, Language>("CategoryTemplateId", (p, l) => p.CategoryTemplateId),
                new PropertyByName<Category, Language>("MetaKeywords", (p, l) => p.MetaKeywords, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("MetaDescription", (p, l) => p.MetaDescription, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("MetaTitle", (p, l) => p.MetaTitle, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("ParentCategoryId", (p, l) => p.ParentCategoryId),
                new PropertyByName<Category, Language>("ParentCategoryName", async (p, l) =>
                {
                    var category = parentCategories.FirstOrDefault(c => c.Id == p.ParentCategoryId);
                    return category != null ? await _categoryService.GetFormattedBreadCrumbAsync(category) : null;

                }, !_catalogSettings.ExportImportCategoriesUsingCategoryName),
                new PropertyByName<Category, Language>("Picture", async (p, l) => await GetPicturesAsync(p.PictureId)),
                new PropertyByName<Category, Language>("PageSize", (p, l) => p.PageSize, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("PriceRangeFiltering", (p, l) => p.PriceRangeFiltering, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("PriceFrom", (p, l) => p.PriceFrom, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("PriceTo", (p, l) => p.PriceTo, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("ManuallyPriceRange", (p, l) => p.ManuallyPriceRange, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("AllowUsersToSelectPageSize", (p, l) => p.AllowUsersToSelectPageSize, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("PageSizeOptions", (p, l) => p.PageSizeOptions, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("ShowOnHomepage", (p, l) => p.ShowOnHomepage, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("IncludeInTopMenu", (p, l) => p.IncludeInTopMenu, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("Published", (p, l) => p.Published, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category, Language>("DisplayOrder", (p, l) => p.DisplayOrder)
            }, _catalogSettings, localizedProperties, languages);

            //activity log
            await _userActivityService.InsertActivityAsync("ExportCategories",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCategories"), categories.Count));

            return await manager.ExportToXlsxAsync(categories);
        }

        /// <summary>
        /// Export tvChannel list to XML
        /// </summary>
        /// <param name="tvChannels">TvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportTvChannelsToXmlAsync(IList<TvChannel> tvChannels)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("TvChannels");
            await xmlWriter.WriteAttributeStringAsync("Version", TvProgVersion.CURRENT_VERSION);
            var currentVendor = await _workContext.GetCurrentVendorAsync();

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            foreach (var tvChannel in tvChannels)
            {
                await xmlWriter.WriteStartElementAsync("TvChannel");

                await xmlWriter.WriteStringAsync("TvChannelId", tvChannel.Id);
                await xmlWriter.WriteStringAsync("TvChannelTypeId", tvChannel.TvChannelTypeId, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelType));
                await xmlWriter.WriteStringAsync("ParentGroupedTvChannelId", tvChannel.ParentGroupedTvChannelId, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelType));
                await xmlWriter.WriteStringAsync("VisibleIndividually", tvChannel.VisibleIndividually, await IgnoreExportTvChannelPropertyAsync(p => p.VisibleIndividually));
                await WriteLocalizedPropertyXmlAsync(tvChannel, p => p.Name, xmlWriter, languages);
                await WriteLocalizedPropertyXmlAsync(tvChannel, p => p.ShortDescription, xmlWriter, languages);
                await WriteLocalizedPropertyXmlAsync(tvChannel, p => p.FullDescription, xmlWriter, languages);
                await xmlWriter.WriteStringAsync("AdminComment", tvChannel.AdminComment, await IgnoreExportTvChannelPropertyAsync(p => p.AdminComment));
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("VendorId", tvChannel.VendorId, await IgnoreExportTvChannelPropertyAsync(p => p.Vendor) || currentVendor != null);
                await xmlWriter.WriteStringAsync("TvChannelTemplateId", tvChannel.TvChannelTemplateId, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelTemplate));
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("ShowOnHomepage", tvChannel.ShowOnHomepage, await IgnoreExportTvChannelPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null);
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("DisplayOrder", tvChannel.DisplayOrder, await IgnoreExportTvChannelPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null);
                await WriteLocalizedPropertyXmlAsync(tvChannel, p => p.MetaKeywords, xmlWriter, languages, await IgnoreExportTvChannelPropertyAsync(p => p.Seo));
                await WriteLocalizedPropertyXmlAsync(tvChannel, p => p.MetaDescription, xmlWriter, languages, await IgnoreExportTvChannelPropertyAsync(p => p.Seo));
                await WriteLocalizedPropertyXmlAsync(tvChannel, p => p.MetaTitle, xmlWriter, languages, await IgnoreExportTvChannelPropertyAsync(p => p.Seo));
                await WriteLocalizedSeNameXmlAsync(tvChannel, xmlWriter, languages, await IgnoreExportTvChannelPropertyAsync(p => p.Seo));
                await xmlWriter.WriteStringAsync("AllowUserReviews", tvChannel.AllowUserReviews, await IgnoreExportTvChannelPropertyAsync(p => p.AllowUserReviews));
                await xmlWriter.WriteStringAsync("SKU", tvChannel.Sku);
                await xmlWriter.WriteStringAsync("ManufacturerPartNumber", tvChannel.ManufacturerPartNumber, await IgnoreExportTvChannelPropertyAsync(p => p.ManufacturerPartNumber));
                await xmlWriter.WriteStringAsync("Gtin", tvChannel.Gtin, await IgnoreExportTvChannelPropertyAsync(p => p.GTIN));
                await xmlWriter.WriteStringAsync("IsGiftCard", tvChannel.IsGiftCard, await IgnoreExportTvChannelPropertyAsync(p => p.IsGiftCard));
                await xmlWriter.WriteStringAsync("GiftCardType", tvChannel.GiftCardType, await IgnoreExportTvChannelPropertyAsync(p => p.IsGiftCard));
                await xmlWriter.WriteStringAsync("OverriddenGiftCardAmount", tvChannel.OverriddenGiftCardAmount, await IgnoreExportTvChannelPropertyAsync(p => p.IsGiftCard));
                await xmlWriter.WriteStringAsync("RequireOtherTvChannels", tvChannel.RequireOtherTvChannels, await IgnoreExportTvChannelPropertyAsync(p => p.RequireOtherTvChannelsAddedToCart));
                await xmlWriter.WriteStringAsync("RequiredTvChannelIds", tvChannel.RequiredTvChannelIds, await IgnoreExportTvChannelPropertyAsync(p => p.RequireOtherTvChannelsAddedToCart));
                await xmlWriter.WriteStringAsync("AutomaticallyAddRequiredTvChannels", tvChannel.AutomaticallyAddRequiredTvChannels, await IgnoreExportTvChannelPropertyAsync(p => p.RequireOtherTvChannelsAddedToCart));
                await xmlWriter.WriteStringAsync("IsDownload", tvChannel.IsDownload, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("DownloadId", tvChannel.DownloadId, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("UnlimitedDownloads", tvChannel.UnlimitedDownloads, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("MaxNumberOfDownloads", tvChannel.MaxNumberOfDownloads, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("DownloadExpirationDays", tvChannel.DownloadExpirationDays, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("DownloadActivationType", tvChannel.DownloadActivationType, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("HasSampleDownload", tvChannel.HasSampleDownload, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("SampleDownloadId", tvChannel.SampleDownloadId, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("HasUserAgreement", tvChannel.HasUserAgreement, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("UserAgreementText", tvChannel.UserAgreementText, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel));
                await xmlWriter.WriteStringAsync("IsRecurring", tvChannel.IsRecurring, await IgnoreExportTvChannelPropertyAsync(p => p.RecurringTvChannel));
                await xmlWriter.WriteStringAsync("RecurringCycleLength", tvChannel.RecurringCycleLength, await IgnoreExportTvChannelPropertyAsync(p => p.RecurringTvChannel));
                await xmlWriter.WriteStringAsync("RecurringCyclePeriodId", tvChannel.RecurringCyclePeriodId, await IgnoreExportTvChannelPropertyAsync(p => p.RecurringTvChannel));
                await xmlWriter.WriteStringAsync("RecurringTotalCycles", tvChannel.RecurringTotalCycles, await IgnoreExportTvChannelPropertyAsync(p => p.RecurringTvChannel));
                await xmlWriter.WriteStringAsync("IsRental", tvChannel.IsRental, await IgnoreExportTvChannelPropertyAsync(p => p.IsRental));
                await xmlWriter.WriteStringAsync("RentalPriceLength", tvChannel.RentalPriceLength, await IgnoreExportTvChannelPropertyAsync(p => p.IsRental));
                await xmlWriter.WriteStringAsync("RentalPricePeriodId", tvChannel.RentalPricePeriodId, await IgnoreExportTvChannelPropertyAsync(p => p.IsRental));
                await xmlWriter.WriteStringAsync("IsShipEnabled", tvChannel.IsShipEnabled);
                await xmlWriter.WriteStringAsync("IsFreeShipping", tvChannel.IsFreeShipping, await IgnoreExportTvChannelPropertyAsync(p => p.FreeShipping));
                await xmlWriter.WriteStringAsync("ShipSeparately", tvChannel.ShipSeparately, await IgnoreExportTvChannelPropertyAsync(p => p.ShipSeparately));
                await xmlWriter.WriteStringAsync("AdditionalShippingCharge", tvChannel.AdditionalShippingCharge, await IgnoreExportTvChannelPropertyAsync(p => p.AdditionalShippingCharge));
                await xmlWriter.WriteStringAsync("DeliveryDateId", tvChannel.DeliveryDateId, await IgnoreExportTvChannelPropertyAsync(p => p.DeliveryDate));
                await xmlWriter.WriteStringAsync("IsTaxExempt", tvChannel.IsTaxExempt);
                await xmlWriter.WriteStringAsync("TaxCategoryId", tvChannel.TaxCategoryId);
                await xmlWriter.WriteStringAsync("IsTelecommunicationsOrBroadcastingOrElectronicServices", tvChannel.IsTelecommunicationsOrBroadcastingOrElectronicServices, await IgnoreExportTvChannelPropertyAsync(p => p.TelecommunicationsBroadcastingElectronicServices));
                await xmlWriter.WriteStringAsync("ManageInventoryMethodId", tvChannel.ManageInventoryMethodId);
                await xmlWriter.WriteStringAsync("TvChannelAvailabilityRangeId", tvChannel.TvChannelAvailabilityRangeId, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelAvailabilityRange));
                await xmlWriter.WriteStringAsync("UseMultipleWarehouses", tvChannel.UseMultipleWarehouses, await IgnoreExportTvChannelPropertyAsync(p => p.UseMultipleWarehouses));
                await xmlWriter.WriteStringAsync("WarehouseId", tvChannel.WarehouseId, await IgnoreExportTvChannelPropertyAsync(p => p.Warehouse));
                await xmlWriter.WriteStringAsync("StockQuantity", tvChannel.StockQuantity);
                await xmlWriter.WriteStringAsync("DisplayStockAvailability", tvChannel.DisplayStockAvailability, await IgnoreExportTvChannelPropertyAsync(p => p.DisplayStockAvailability));
                await xmlWriter.WriteStringAsync("DisplayStockQuantity", tvChannel.DisplayStockQuantity, await IgnoreExportTvChannelPropertyAsync(p => p.DisplayStockAvailability));
                await xmlWriter.WriteStringAsync("MinStockQuantity", tvChannel.MinStockQuantity, await IgnoreExportTvChannelPropertyAsync(p => p.MinimumStockQuantity));
                await xmlWriter.WriteStringAsync("LowStockActivityId", tvChannel.LowStockActivityId, await IgnoreExportTvChannelPropertyAsync(p => p.LowStockActivity));
                await xmlWriter.WriteStringAsync("NotifyAdminForQuantityBelow", tvChannel.NotifyAdminForQuantityBelow, await IgnoreExportTvChannelPropertyAsync(p => p.NotifyAdminForQuantityBelow));
                await xmlWriter.WriteStringAsync("BackorderModeId", tvChannel.BackorderModeId, await IgnoreExportTvChannelPropertyAsync(p => p.Backorders));
                await xmlWriter.WriteStringAsync("AllowBackInStockSubscriptions", tvChannel.AllowBackInStockSubscriptions, await IgnoreExportTvChannelPropertyAsync(p => p.AllowBackInStockSubscriptions));
                await xmlWriter.WriteStringAsync("OrderMinimumQuantity", tvChannel.OrderMinimumQuantity, await IgnoreExportTvChannelPropertyAsync(p => p.MinimumCartQuantity));
                await xmlWriter.WriteStringAsync("OrderMaximumQuantity", tvChannel.OrderMaximumQuantity, await IgnoreExportTvChannelPropertyAsync(p => p.MaximumCartQuantity));
                await xmlWriter.WriteStringAsync("AllowedQuantities", tvChannel.AllowedQuantities, await IgnoreExportTvChannelPropertyAsync(p => p.AllowedQuantities));
                await xmlWriter.WriteStringAsync("AllowAddingOnlyExistingAttributeCombinations", tvChannel.AllowAddingOnlyExistingAttributeCombinations, await IgnoreExportTvChannelPropertyAsync(p => p.AllowAddingOnlyExistingAttributeCombinations));
                await xmlWriter.WriteStringAsync("NotReturnable", tvChannel.NotReturnable, await IgnoreExportTvChannelPropertyAsync(p => p.NotReturnable));
                await xmlWriter.WriteStringAsync("DisableBuyButton", tvChannel.DisableBuyButton, await IgnoreExportTvChannelPropertyAsync(p => p.DisableBuyButton));
                await xmlWriter.WriteStringAsync("DisableWishlistButton", tvChannel.DisableWishlistButton, await IgnoreExportTvChannelPropertyAsync(p => p.DisableWishlistButton));
                await xmlWriter.WriteStringAsync("AvailableForPreOrder", tvChannel.AvailableForPreOrder, await IgnoreExportTvChannelPropertyAsync(p => p.AvailableForPreOrder));
                await xmlWriter.WriteStringAsync("PreOrderAvailabilityStartDateTimeUtc", tvChannel.PreOrderAvailabilityStartDateTimeUtc, await IgnoreExportTvChannelPropertyAsync(p => p.AvailableForPreOrder));
                await xmlWriter.WriteStringAsync("CallForPrice", tvChannel.CallForPrice, await IgnoreExportTvChannelPropertyAsync(p => p.CallForPrice));
                await xmlWriter.WriteStringAsync("Price", tvChannel.Price);
                await xmlWriter.WriteStringAsync("OldPrice", tvChannel.OldPrice, await IgnoreExportTvChannelPropertyAsync(p => p.OldPrice));
                await xmlWriter.WriteStringAsync("TvChannelCost", tvChannel.TvChannelCost, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelCost));
                await xmlWriter.WriteStringAsync("UserEntersPrice", tvChannel.UserEntersPrice, await IgnoreExportTvChannelPropertyAsync(p => p.UserEntersPrice));
                await xmlWriter.WriteStringAsync("MinimumUserEnteredPrice", tvChannel.MinimumUserEnteredPrice, await IgnoreExportTvChannelPropertyAsync(p => p.UserEntersPrice));
                await xmlWriter.WriteStringAsync("MaximumUserEnteredPrice", tvChannel.MaximumUserEnteredPrice, await IgnoreExportTvChannelPropertyAsync(p => p.UserEntersPrice));
                await xmlWriter.WriteStringAsync("BasepriceEnabled", tvChannel.BasepriceEnabled, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV));
                await xmlWriter.WriteStringAsync("BasepriceAmount", tvChannel.BasepriceAmount, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV));
                await xmlWriter.WriteStringAsync("BasepriceUnitId", tvChannel.BasepriceUnitId, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV));
                await xmlWriter.WriteStringAsync("BasepriceBaseAmount", tvChannel.BasepriceBaseAmount, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV));
                await xmlWriter.WriteStringAsync("BasepriceBaseUnitId", tvChannel.BasepriceBaseUnitId, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV));
                await xmlWriter.WriteStringAsync("MarkAsNew", tvChannel.MarkAsNew, await IgnoreExportTvChannelPropertyAsync(p => p.MarkAsNew));
                await xmlWriter.WriteStringAsync("MarkAsNewStartDateTimeUtc", tvChannel.MarkAsNewStartDateTimeUtc, await IgnoreExportTvChannelPropertyAsync(p => p.MarkAsNew));
                await xmlWriter.WriteStringAsync("MarkAsNewEndDateTimeUtc", tvChannel.MarkAsNewEndDateTimeUtc, await IgnoreExportTvChannelPropertyAsync(p => p.MarkAsNew));
                await xmlWriter.WriteStringAsync("Weight", tvChannel.Weight, await IgnoreExportTvChannelPropertyAsync(p => p.Weight));
                await xmlWriter.WriteStringAsync("Length", tvChannel.Length, await IgnoreExportTvChannelPropertyAsync(p => p.Dimensions));
                await xmlWriter.WriteStringAsync("Width", tvChannel.Width, await IgnoreExportTvChannelPropertyAsync(p => p.Dimensions));
                await xmlWriter.WriteStringAsync("Height", tvChannel.Height, await IgnoreExportTvChannelPropertyAsync(p => p.Dimensions));
                await xmlWriter.WriteStringAsync("Published", tvChannel.Published, await IgnoreExportTvChannelPropertyAsync(p => p.Published));
                await xmlWriter.WriteStringAsync("CreatedOnUtc", tvChannel.CreatedOnUtc);
                await xmlWriter.WriteStringAsync("UpdatedOnUtc", tvChannel.UpdatedOnUtc);

                if (!await IgnoreExportTvChannelPropertyAsync(p => p.Discounts))
                {
                    await xmlWriter.WriteStartElementAsync("TvChannelDiscounts");

                    foreach (var discount in await _discountService.GetAppliedDiscountsAsync(tvChannel))
                    {
                        await xmlWriter.WriteStartElementAsync("Discount");
                        await xmlWriter.WriteStringAsync("DiscountId", discount.Id);
                        await xmlWriter.WriteStringAsync("Name", discount.Name);
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                }

                if (!await IgnoreExportTvChannelPropertyAsync(p => p.TierPrices))
                {
                    await xmlWriter.WriteStartElementAsync("TierPrices");
                    var tierPrices = await _tvChannelService.GetTierPricesByTvChannelAsync(tvChannel.Id);
                    foreach (var tierPrice in tierPrices)
                    {
                        await xmlWriter.WriteStartElementAsync("TierPrice");
                        await xmlWriter.WriteStringAsync("TierPriceId", tierPrice.Id);
                        await xmlWriter.WriteStringAsync("StoreId", tierPrice.StoreId);
                        await xmlWriter.WriteStringAsync("UserRoleId", tierPrice.UserRoleId, defaulValue: "0");
                        await xmlWriter.WriteStringAsync("Quantity", tierPrice.Quantity);
                        await xmlWriter.WriteStringAsync("Price", tierPrice.Price);
                        await xmlWriter.WriteStringAsync("StartDateTimeUtc", tierPrice.StartDateTimeUtc);
                        await xmlWriter.WriteStringAsync("EndDateTimeUtc", tierPrice.EndDateTimeUtc);
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                }

                if (!await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelAttributes))
                {
                    await xmlWriter.WriteStartElementAsync("TvChannelAttributes");
                    var tvChannelAttributMappings =
                        await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id);
                    foreach (var tvChannelAttributeMapping in tvChannelAttributMappings)
                    {
                        var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(tvChannelAttributeMapping.TvChannelAttributeId);

                        await xmlWriter.WriteStartElementAsync("TvChannelAttributeMapping");
                        await xmlWriter.WriteStringAsync("TvChannelAttributeMappingId", tvChannelAttributeMapping.Id);
                        await xmlWriter.WriteStringAsync("TvChannelAttributeId", tvChannelAttributeMapping.TvChannelAttributeId);
                        await xmlWriter.WriteStringAsync("TvChannelAttributeName", tvChannelAttribute.Name);
                        await WriteLocalizedPropertyXmlAsync(tvChannelAttributeMapping, pam => pam.TextPrompt, xmlWriter, languages, overriddenNodeName: "TextPrompt");
                        await xmlWriter.WriteStringAsync("IsRequired", tvChannelAttributeMapping.IsRequired);
                        await xmlWriter.WriteStringAsync("AttributeControlTypeId", tvChannelAttributeMapping.AttributeControlTypeId);
                        await xmlWriter.WriteStringAsync("DisplayOrder", tvChannelAttributeMapping.DisplayOrder);
                        //validation rules
                        if (tvChannelAttributeMapping.ValidationRulesAllowed())
                        {
                            if (tvChannelAttributeMapping.ValidationMinLength.HasValue)
                            {
                                await xmlWriter.WriteStringAsync("ValidationMinLength",
                                    tvChannelAttributeMapping.ValidationMinLength.Value);
                            }

                            if (tvChannelAttributeMapping.ValidationMaxLength.HasValue)
                            {
                                await xmlWriter.WriteStringAsync("ValidationMaxLength",
                                    tvChannelAttributeMapping.ValidationMaxLength.Value);
                            }

                            if (string.IsNullOrEmpty(tvChannelAttributeMapping.ValidationFileAllowedExtensions))
                            {
                                await xmlWriter.WriteStringAsync("ValidationFileAllowedExtensions",
                                    tvChannelAttributeMapping.ValidationFileAllowedExtensions);
                            }

                            if (tvChannelAttributeMapping.ValidationFileMaximumSize.HasValue)
                            {
                                await xmlWriter.WriteStringAsync("ValidationFileMaximumSize",
                                    tvChannelAttributeMapping.ValidationFileMaximumSize.Value);
                            }

                            await WriteLocalizedPropertyXmlAsync(tvChannelAttributeMapping, pam => pam.DefaultValue, xmlWriter, languages, overriddenNodeName: "DefaultValue");
                        }
                        //conditions
                        await xmlWriter.WriteElementStringAsync("ConditionAttributeXml", tvChannelAttributeMapping.ConditionAttributeXml);

                        await xmlWriter.WriteStartElementAsync("TvChannelAttributeValues");
                        var tvChannelAttributeValues = await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(tvChannelAttributeMapping.Id);
                        foreach (var tvChannelAttributeValue in tvChannelAttributeValues)
                        {
                            await xmlWriter.WriteStartElementAsync("TvChannelAttributeValue");
                            await xmlWriter.WriteStringAsync("TvChannelAttributeValueId", tvChannelAttributeValue.Id);
                            await WriteLocalizedPropertyXmlAsync(tvChannelAttributeValue, pav => pav.Name, xmlWriter, languages, overriddenNodeName: "Name");
                            await xmlWriter.WriteStringAsync("AttributeValueTypeId", tvChannelAttributeValue.AttributeValueTypeId);
                            await xmlWriter.WriteStringAsync("AssociatedTvChannelId", tvChannelAttributeValue.AssociatedTvChannelId);
                            await xmlWriter.WriteStringAsync("ColorSquaresRgb", tvChannelAttributeValue.ColorSquaresRgb);
                            await xmlWriter.WriteStringAsync("ImageSquaresPictureId", tvChannelAttributeValue.ImageSquaresPictureId);
                            await xmlWriter.WriteStringAsync("PriceAdjustment", tvChannelAttributeValue.PriceAdjustment);
                            await xmlWriter.WriteStringAsync("PriceAdjustmentUsePercentage", tvChannelAttributeValue.PriceAdjustmentUsePercentage);
                            await xmlWriter.WriteStringAsync("WeightAdjustment", tvChannelAttributeValue.WeightAdjustment);
                            await xmlWriter.WriteStringAsync("Cost", tvChannelAttributeValue.Cost);
                            await xmlWriter.WriteStringAsync("UserEntersQty", tvChannelAttributeValue.UserEntersQty);
                            await xmlWriter.WriteStringAsync("Quantity", tvChannelAttributeValue.Quantity);
                            await xmlWriter.WriteStringAsync("IsPreSelected", tvChannelAttributeValue.IsPreSelected);
                            await xmlWriter.WriteStringAsync("DisplayOrder", tvChannelAttributeValue.DisplayOrder);
                            await xmlWriter.WriteStringAsync("PictureId", tvChannelAttributeValue.PictureId);
                            await xmlWriter.WriteEndElementAsync();
                        }

                        await xmlWriter.WriteEndElementAsync();
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteStartElementAsync("TvChannelPictures");
                var tvChannelPictures = await _tvChannelService.GetTvChannelPicturesByTvChannelIdAsync(tvChannel.Id);
                foreach (var tvChannelPicture in tvChannelPictures)
                {
                    await xmlWriter.WriteStartElementAsync("TvChannelPicture");
                    await xmlWriter.WriteStringAsync("TvChannelPictureId", tvChannelPicture.Id);
                    await xmlWriter.WriteStringAsync("PictureId", tvChannelPicture.PictureId);
                    await xmlWriter.WriteStringAsync("DisplayOrder", tvChannelPicture.DisplayOrder);
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();

                await xmlWriter.WriteStartElementAsync("TvChannelCategories");
                var tvChannelCategories = await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvChannel.Id, true);
                if (tvChannelCategories != null)
                {
                    foreach (var tvChannelCategory in tvChannelCategories)
                    {
                        await xmlWriter.WriteStartElementAsync("TvChannelCategory");
                        await xmlWriter.WriteStringAsync("TvChannelCategoryId", tvChannelCategory.Id);
                        await xmlWriter.WriteStringAsync("CategoryId", tvChannelCategory.CategoryId);
                        await xmlWriter.WriteStringAsync("IsFeaturedTvChannel", tvChannelCategory.IsFeaturedTvChannel);
                        await xmlWriter.WriteStringAsync("DisplayOrder", tvChannelCategory.DisplayOrder);
                        await xmlWriter.WriteEndElementAsync();
                    }
                }

                await xmlWriter.WriteEndElementAsync();

                if (!await IgnoreExportTvChannelPropertyAsync(p => p.Manufacturers))
                {
                    await xmlWriter.WriteStartElementAsync("TvChannelManufacturers");
                    var tvChannelManufacturers = await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvChannel.Id);
                    if (tvChannelManufacturers != null)
                    {
                        foreach (var tvChannelManufacturer in tvChannelManufacturers)
                        {
                            await xmlWriter.WriteStartElementAsync("TvChannelManufacturer");
                            await xmlWriter.WriteStringAsync("TvChannelManufacturerId", tvChannelManufacturer.Id);
                            await xmlWriter.WriteStringAsync("ManufacturerId", tvChannelManufacturer.ManufacturerId);
                            await xmlWriter.WriteStringAsync("IsFeaturedTvChannel", tvChannelManufacturer.IsFeaturedTvChannel);
                            await xmlWriter.WriteStringAsync("DisplayOrder", tvChannelManufacturer.DisplayOrder);
                            await xmlWriter.WriteEndElementAsync();
                        }
                    }

                    await xmlWriter.WriteEndElementAsync();
                }

                if (!await IgnoreExportTvChannelPropertyAsync(p => p.SpecificationAttributes))
                {
                    await xmlWriter.WriteStartElementAsync("TvChannelSpecificationAttributes");
                    var tvChannelSpecificationAttributes = await _specificationAttributeService.GetTvChannelSpecificationAttributesAsync(tvChannel.Id);
                    foreach (var tvChannelSpecificationAttribute in tvChannelSpecificationAttributes)
                    {
                        await xmlWriter.WriteStartElementAsync("TvChannelSpecificationAttribute");
                        await xmlWriter.WriteStringAsync("TvChannelSpecificationAttributeId", tvChannelSpecificationAttribute.Id);
                        await xmlWriter.WriteStringAsync("SpecificationAttributeOptionId", tvChannelSpecificationAttribute.SpecificationAttributeOptionId);
                        await xmlWriter.WriteStringAsync("CustomValue", tvChannelSpecificationAttribute.CustomValue);
                        await xmlWriter.WriteStringAsync("AllowFiltering", tvChannelSpecificationAttribute.AllowFiltering);
                        await xmlWriter.WriteStringAsync("ShowOnTvChannelPage", tvChannelSpecificationAttribute.ShowOnTvChannelPage);
                        await xmlWriter.WriteStringAsync("DisplayOrder", tvChannelSpecificationAttribute.DisplayOrder);
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                }

                if (!await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelTags))
                {
                    await xmlWriter.WriteStartElementAsync("TvChannelTags");
                    var tvChannelTags = await _tvChannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvChannel.Id);
                    foreach (var tvChannelTag in tvChannelTags)
                    {
                        await xmlWriter.WriteStartElementAsync("TvChannelTag");
                        await xmlWriter.WriteStringAsync("Id", tvChannelTag.Id);
                        await xmlWriter.WriteStringAsync("Name", tvChannelTag.Name);
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            //activity log
            await _userActivityService.InsertActivityAsync("ExportTvChannels",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportTvChannels"), tvChannels.Count));

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export tvChannels to XLSX
        /// </summary>
        /// <param name="tvChannels">TvChannels</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<byte[]> ExportTvChannelsToXlsxAsync(IEnumerable<TvChannel> tvChannels)
        {
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            var localizedProperties = new[]
            {
                new PropertyByName<TvChannel, Language>("TvChannelId", (p, l) => p.Id),
                new PropertyByName<TvChannel, Language>("Name", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.Name, l.Id, false)),
                new PropertyByName<TvChannel, Language>("ShortDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.ShortDescription, l.Id, false)),
                new PropertyByName<TvChannel, Language>("FullDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.FullDescription, l.Id, false)),
                new PropertyByName<TvChannel, Language>("MetaKeywords", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaKeywords, l.Id, false)),
                new PropertyByName<TvChannel, Language>("MetaDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaDescription, l.Id, false)),
                new PropertyByName<TvChannel, Language>("MetaTitle", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaTitle, l.Id, false)),
                new PropertyByName<TvChannel, Language>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, l.Id, returnDefaultValue: false), await IgnoreExportTvChannelPropertyAsync(p => p.Seo))
            };

            var properties = new[]
            {
                new PropertyByName<TvChannel, Language>("TvChannelId", (p, l) => p.Id),
                new PropertyByName<TvChannel, Language>("TvChannelType", (p, l) => p.TvChannelTypeId, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelType))
                {
                    DropDownElements = await TvChannelType.SimpleTvChannel.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<TvChannel, Language>("ParentGroupedTvChannelId", (p, l) => p.ParentGroupedTvChannelId, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelType)),
                new PropertyByName<TvChannel, Language>("VisibleIndividually", (p, l) => p.VisibleIndividually, await IgnoreExportTvChannelPropertyAsync(p => p.VisibleIndividually)),
                new PropertyByName<TvChannel, Language>("Name", (p, l) => p.Name),
                new PropertyByName<TvChannel, Language>("ShortDescription", (p, l) => p.ShortDescription),
                new PropertyByName<TvChannel, Language>("FullDescription", (p, l) => p.FullDescription),
                //vendor can't change this field
                new PropertyByName<TvChannel, Language>("Vendor", (p, l) => p.VendorId, await IgnoreExportTvChannelPropertyAsync(p => p.Vendor) || currentVendor != null)
                {
                    DropDownElements = (await _vendorService.GetAllVendorsAsync(showHidden: true)).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<TvChannel, Language>("TvChannelTemplate", (p, l) => p.TvChannelTemplateId, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelTemplate))
                {
                    DropDownElements = (await _tvChannelTemplateService.GetAllTvChannelTemplatesAsync()).Select(pt => pt as BaseEntity).ToSelectList(p => (p as TvChannelTemplate)?.Name ?? string.Empty)
                },
                //vendor can't change this field
                new PropertyByName<TvChannel, Language>("ShowOnHomepage", (p, l) => p.ShowOnHomepage, await IgnoreExportTvChannelPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null),
                //vendor can't change this field
                new PropertyByName<TvChannel, Language>("DisplayOrder", (p, l) => p.DisplayOrder, await IgnoreExportTvChannelPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null),
                new PropertyByName<TvChannel, Language>("MetaKeywords", (p, l) => p.MetaKeywords, await IgnoreExportTvChannelPropertyAsync(p => p.Seo)),
                new PropertyByName<TvChannel, Language>("MetaDescription", (p, l) => p.MetaDescription, await IgnoreExportTvChannelPropertyAsync(p => p.Seo)),
                new PropertyByName<TvChannel, Language>("MetaTitle", (p, l) => p.MetaTitle, await IgnoreExportTvChannelPropertyAsync(p => p.Seo)),
                new PropertyByName<TvChannel, Language>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportTvChannelPropertyAsync(p => p.Seo)),
                new PropertyByName<TvChannel, Language>("AllowUserReviews", (p, l) => p.AllowUserReviews, await IgnoreExportTvChannelPropertyAsync(p => p.AllowUserReviews)),
                new PropertyByName<TvChannel, Language>("Published", (p, l) => p.Published, await IgnoreExportTvChannelPropertyAsync(p => p.Published)),
                new PropertyByName<TvChannel, Language>("SKU", (p, l) => p.Sku),
                new PropertyByName<TvChannel, Language>("ManufacturerPartNumber", (p, l) => p.ManufacturerPartNumber, await IgnoreExportTvChannelPropertyAsync(p => p.ManufacturerPartNumber)),
                new PropertyByName<TvChannel, Language>("Gtin", (p, l) => p.Gtin, await IgnoreExportTvChannelPropertyAsync(p => p.GTIN)),
                new PropertyByName<TvChannel, Language>("IsGiftCard", (p, l) => p.IsGiftCard, await IgnoreExportTvChannelPropertyAsync(p => p.IsGiftCard)),
                new PropertyByName<TvChannel, Language>("GiftCardType", (p, l) => p.GiftCardTypeId, await IgnoreExportTvChannelPropertyAsync(p => p.IsGiftCard))
                {
                    DropDownElements = await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<TvChannel, Language>("OverriddenGiftCardAmount", (p, l) => p.OverriddenGiftCardAmount, await IgnoreExportTvChannelPropertyAsync(p => p.IsGiftCard)),
                new PropertyByName<TvChannel, Language>("RequireOtherTvChannels", (p, l) => p.RequireOtherTvChannels, await IgnoreExportTvChannelPropertyAsync(p => p.RequireOtherTvChannelsAddedToCart)),
                new PropertyByName<TvChannel, Language>("RequiredTvChannelIds", (p, l) => p.RequiredTvChannelIds, await IgnoreExportTvChannelPropertyAsync(p => p.RequireOtherTvChannelsAddedToCart)),
                new PropertyByName<TvChannel, Language>("AutomaticallyAddRequiredTvChannels", (p, l) => p.AutomaticallyAddRequiredTvChannels, await IgnoreExportTvChannelPropertyAsync(p => p.RequireOtherTvChannelsAddedToCart)),
                new PropertyByName<TvChannel, Language>("IsDownload", (p, l) => p.IsDownload, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel)),
                new PropertyByName<TvChannel, Language>("DownloadId", (p, l) => p.DownloadId, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel)),
                new PropertyByName<TvChannel, Language>("UnlimitedDownloads", (p, l) => p.UnlimitedDownloads, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel)),
                new PropertyByName<TvChannel, Language>("MaxNumberOfDownloads", (p, l) => p.MaxNumberOfDownloads, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel)),
                new PropertyByName<TvChannel, Language>("DownloadActivationType", (p, l) => p.DownloadActivationTypeId, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel))
                {
                    DropDownElements = await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<TvChannel, Language>("HasSampleDownload", (p, l) => p.HasSampleDownload, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel)),
                new PropertyByName<TvChannel, Language>("SampleDownloadId", (p, l) => p.SampleDownloadId, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel)),
                new PropertyByName<TvChannel, Language>("HasUserAgreement", (p, l) => p.HasUserAgreement, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel)),
                new PropertyByName<TvChannel, Language>("UserAgreementText", (p, l) => p.UserAgreementText, await IgnoreExportTvChannelPropertyAsync(p => p.DownloadableTvChannel)),
                new PropertyByName<TvChannel, Language>("IsRecurring", (p, l) => p.IsRecurring, await IgnoreExportTvChannelPropertyAsync(p => p.RecurringTvChannel)),
                new PropertyByName<TvChannel, Language>("RecurringCycleLength", (p, l) => p.RecurringCycleLength, await IgnoreExportTvChannelPropertyAsync(p => p.RecurringTvChannel)),
                new PropertyByName<TvChannel, Language>("RecurringCyclePeriod", (p, l) => p.RecurringCyclePeriodId, await IgnoreExportTvChannelPropertyAsync(p => p.RecurringTvChannel))
                {
                    DropDownElements = await RecurringTvChannelCyclePeriod.Days.ToSelectListAsync(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<TvChannel, Language>("RecurringTotalCycles", (p, l) => p.RecurringTotalCycles, await IgnoreExportTvChannelPropertyAsync(p => p.RecurringTvChannel)),
                new PropertyByName<TvChannel, Language>("IsRental", (p, l) => p.IsRental, await IgnoreExportTvChannelPropertyAsync(p => p.IsRental)),
                new PropertyByName<TvChannel, Language>("RentalPriceLength", (p, l) => p.RentalPriceLength, await IgnoreExportTvChannelPropertyAsync(p => p.IsRental)),
                new PropertyByName<TvChannel, Language>("RentalPricePeriod", (p, l) => p.RentalPricePeriodId, await IgnoreExportTvChannelPropertyAsync(p => p.IsRental))
                {
                    DropDownElements = await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<TvChannel, Language>("IsShipEnabled", (p, l) => p.IsShipEnabled),
                new PropertyByName<TvChannel, Language>("IsFreeShipping", (p, l) => p.IsFreeShipping, await IgnoreExportTvChannelPropertyAsync(p => p.FreeShipping)),
                new PropertyByName<TvChannel, Language>("ShipSeparately", (p, l) => p.ShipSeparately, await IgnoreExportTvChannelPropertyAsync(p => p.ShipSeparately)),
                new PropertyByName<TvChannel, Language>("AdditionalShippingCharge", (p, l) => p.AdditionalShippingCharge, await IgnoreExportTvChannelPropertyAsync(p => p.AdditionalShippingCharge)),
                new PropertyByName<TvChannel, Language>("DeliveryDate", (p, l) => p.DeliveryDateId, await IgnoreExportTvChannelPropertyAsync(p => p.DeliveryDate))
                {
                    DropDownElements = (await _dateRangeService.GetAllDeliveryDatesAsync()).Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<TvChannel, Language>("IsTaxExempt", (p, l) => p.IsTaxExempt),
                new PropertyByName<TvChannel, Language>("TaxCategory", (p, l) => p.TaxCategoryId)
                {
                    DropDownElements = (await _taxCategoryService.GetAllTaxCategoriesAsync()).Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<TvChannel, Language>("IsTelecommunicationsOrBroadcastingOrElectronicServices", (p, l) => p.IsTelecommunicationsOrBroadcastingOrElectronicServices, await IgnoreExportTvChannelPropertyAsync(p => p.TelecommunicationsBroadcastingElectronicServices)),
                new PropertyByName<TvChannel, Language>("ManageInventoryMethod", (p, l) => p.ManageInventoryMethodId)
                {
                    DropDownElements = await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<TvChannel, Language>("TvChannelAvailabilityRange", (p, l) => p.TvChannelAvailabilityRangeId, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelAvailabilityRange))
                {
                    DropDownElements = (await _dateRangeService.GetAllTvChannelAvailabilityRangesAsync()).Select(range => range as BaseEntity).ToSelectList(p => (p as TvChannelAvailabilityRange)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<TvChannel, Language>("UseMultipleWarehouses", (p, l) => p.UseMultipleWarehouses, await IgnoreExportTvChannelPropertyAsync(p => p.UseMultipleWarehouses)),
                new PropertyByName<TvChannel, Language>("WarehouseId", (p, l) => p.WarehouseId, await IgnoreExportTvChannelPropertyAsync(p => p.Warehouse)),
                new PropertyByName<TvChannel, Language>("StockQuantity", (p, l) => p.StockQuantity),
                new PropertyByName<TvChannel, Language>("DisplayStockAvailability", (p, l) => p.DisplayStockAvailability, await IgnoreExportTvChannelPropertyAsync(p => p.DisplayStockAvailability)),
                new PropertyByName<TvChannel, Language>("DisplayStockQuantity", (p, l) => p.DisplayStockQuantity, await IgnoreExportTvChannelPropertyAsync(p => p.DisplayStockAvailability)),
                new PropertyByName<TvChannel, Language>("MinStockQuantity", (p, l) => p.MinStockQuantity, await IgnoreExportTvChannelPropertyAsync(p => p.MinimumStockQuantity)),
                new PropertyByName<TvChannel, Language>("LowStockActivity", (p, l) => p.LowStockActivityId, await IgnoreExportTvChannelPropertyAsync(p => p.LowStockActivity))
                {
                    DropDownElements = await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<TvChannel, Language>("NotifyAdminForQuantityBelow", (p, l) => p.NotifyAdminForQuantityBelow, await IgnoreExportTvChannelPropertyAsync(p => p.NotifyAdminForQuantityBelow)),
                new PropertyByName<TvChannel, Language>("BackorderMode", (p, l) => p.BackorderModeId, await IgnoreExportTvChannelPropertyAsync(p => p.Backorders))
                {
                    DropDownElements = await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<TvChannel, Language>("AllowBackInStockSubscriptions", (p, l) => p.AllowBackInStockSubscriptions, await IgnoreExportTvChannelPropertyAsync(p => p.AllowBackInStockSubscriptions)),
                new PropertyByName<TvChannel, Language>("OrderMinimumQuantity", (p, l) => p.OrderMinimumQuantity, await IgnoreExportTvChannelPropertyAsync(p => p.MinimumCartQuantity)),
                new PropertyByName<TvChannel, Language>("OrderMaximumQuantity", (p, l) => p.OrderMaximumQuantity, await IgnoreExportTvChannelPropertyAsync(p => p.MaximumCartQuantity)),
                new PropertyByName<TvChannel, Language>("AllowedQuantities", (p, l) => p.AllowedQuantities, await IgnoreExportTvChannelPropertyAsync(p => p.AllowedQuantities)),
                new PropertyByName<TvChannel, Language>("AllowAddingOnlyExistingAttributeCombinations", (p, l) => p.AllowAddingOnlyExistingAttributeCombinations, await IgnoreExportTvChannelPropertyAsync(p => p.AllowAddingOnlyExistingAttributeCombinations)),
                new PropertyByName<TvChannel, Language>("NotReturnable", (p, l) => p.NotReturnable, await IgnoreExportTvChannelPropertyAsync(p => p.NotReturnable)),
                new PropertyByName<TvChannel, Language>("DisableBuyButton", (p, l) => p.DisableBuyButton, await IgnoreExportTvChannelPropertyAsync(p => p.DisableBuyButton)),
                new PropertyByName<TvChannel, Language>("DisableWishlistButton", (p, l) => p.DisableWishlistButton, await IgnoreExportTvChannelPropertyAsync(p => p.DisableWishlistButton)),
                new PropertyByName<TvChannel, Language>("AvailableForPreOrder", (p, l) => p.AvailableForPreOrder, await IgnoreExportTvChannelPropertyAsync(p => p.AvailableForPreOrder)),
                new PropertyByName<TvChannel, Language>("PreOrderAvailabilityStartDateTimeUtc", (p, l) => p.PreOrderAvailabilityStartDateTimeUtc, await IgnoreExportTvChannelPropertyAsync(p => p.AvailableForPreOrder)),
                new PropertyByName<TvChannel, Language>("CallForPrice", (p, l) => p.CallForPrice, await IgnoreExportTvChannelPropertyAsync(p => p.CallForPrice)),
                new PropertyByName<TvChannel, Language>("Price", (p, l) => p.Price),
                new PropertyByName<TvChannel, Language>("OldPrice", (p, l) => p.OldPrice, await IgnoreExportTvChannelPropertyAsync(p => p.OldPrice)),
                new PropertyByName<TvChannel, Language>("TvChannelCost", (p, l) => p.TvChannelCost, await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelCost)),
                new PropertyByName<TvChannel, Language>("UserEntersPrice", (p, l) => p.UserEntersPrice, await IgnoreExportTvChannelPropertyAsync(p => p.UserEntersPrice)),
                new PropertyByName<TvChannel, Language>("MinimumUserEnteredPrice", (p, l) => p.MinimumUserEnteredPrice, await IgnoreExportTvChannelPropertyAsync(p => p.UserEntersPrice)),
                new PropertyByName<TvChannel, Language>("MaximumUserEnteredPrice", (p, l) => p.MaximumUserEnteredPrice, await IgnoreExportTvChannelPropertyAsync(p => p.UserEntersPrice)),
                new PropertyByName<TvChannel, Language>("BasepriceEnabled", (p, l) => p.BasepriceEnabled, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV)),
                new PropertyByName<TvChannel, Language>("BasepriceAmount", (p, l) => p.BasepriceAmount, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV)),
                new PropertyByName<TvChannel, Language>("BasepriceUnit", (p, l) => p.BasepriceUnitId, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV))
                {
                    DropDownElements = (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<TvChannel, Language>("BasepriceBaseAmount", (p, l) => p.BasepriceBaseAmount, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV)),
                new PropertyByName<TvChannel, Language>("BasepriceBaseUnit", (p, l) => p.BasepriceBaseUnitId, await IgnoreExportTvChannelPropertyAsync(p => p.PAngV))
                {
                    DropDownElements = (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<TvChannel, Language>("MarkAsNew", (p, l) => p.MarkAsNew, await IgnoreExportTvChannelPropertyAsync(p => p.MarkAsNew)),
                new PropertyByName<TvChannel, Language>("MarkAsNewStartDateTimeUtc", (p, l) => p.MarkAsNewStartDateTimeUtc, await IgnoreExportTvChannelPropertyAsync(p => p.MarkAsNew)),
                new PropertyByName<TvChannel, Language>("MarkAsNewEndDateTimeUtc", (p, l) => p.MarkAsNewEndDateTimeUtc, await IgnoreExportTvChannelPropertyAsync(p => p.MarkAsNew)),
                new PropertyByName<TvChannel, Language>("Weight", (p, l) => p.Weight, await IgnoreExportTvChannelPropertyAsync(p => p.Weight)),
                new PropertyByName<TvChannel, Language>("Length", (p, l) => p.Length, await IgnoreExportTvChannelPropertyAsync(p => p.Dimensions)),
                new PropertyByName<TvChannel, Language>("Width", (p, l) => p.Width, await IgnoreExportTvChannelPropertyAsync(p => p.Dimensions)),
                new PropertyByName<TvChannel, Language>("Height", (p, l) => p.Height, await IgnoreExportTvChannelPropertyAsync(p => p.Dimensions)),
                new PropertyByName<TvChannel, Language>("Categories", async (p, l) =>  await GetCategoriesAsync(p)),
                new PropertyByName<TvChannel, Language>("Manufacturers", async (p, l) =>  await GetManufacturersAsync(p), await IgnoreExportTvChannelPropertyAsync(p => p.Manufacturers)),
                new PropertyByName<TvChannel, Language>("TvChannelTags", async (p, l) =>  await GetTvChannelTagsAsync(p), await IgnoreExportTvChannelPropertyAsync(p => p.TvChannelTags)),
                new PropertyByName<TvChannel, Language>("IsLimitedToStores", (p, l) => p.LimitedToStores, await IgnoreExportLimitedToStoreAsync()),
                new PropertyByName<TvChannel, Language>("LimitedToStores",async (p, l) =>  await GetLimitedToStoresAsync(p), await IgnoreExportLimitedToStoreAsync()),
                new PropertyByName<TvChannel, Language>("Picture1", async (p, l) => await GetPictureAsync(p, 0)),
                new PropertyByName<TvChannel, Language>("Picture2", async (p, l) => await GetPictureAsync(p, 1)),
                new PropertyByName<TvChannel, Language>("Picture3", async (p, l) => await GetPictureAsync(p, 2))
            };

            var tvChannelList = tvChannels.ToList();

            var tvChannelAdvancedMode = true;
            try
            {
                tvChannelAdvancedMode = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), "tvChannel-advanced-mode");
            }
            catch (ArgumentNullException)
            {
            }

            if (!_catalogSettings.ExportImportTvChannelAttributes && !_catalogSettings.ExportImportTvChannelSpecificationAttributes)
                return await new PropertyManager<TvChannel, Language>(properties, _catalogSettings).ExportToXlsxAsync(tvChannelList);

            //activity log
            await _userActivityService.InsertActivityAsync("ExportTvChannels",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportTvChannels"), tvChannelList.Count));

            if (tvChannelAdvancedMode || _tvChannelEditorSettings.TvChannelAttributes)
                return await ExportTvChannelsToXlsxWithAttributesAsync(properties, localizedProperties, tvChannelList, languages);

            return await new PropertyManager<TvChannel, Language>(properties, _catalogSettings, localizedProperties, languages).ExportToXlsxAsync(tvChannelList);
        }

        /// <summary>
        /// Export order list to XML
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportOrdersToXmlAsync(IList<Order> orders)
        {
            //a vendor should have access only to part of order information
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            var ignore = currentVendor != null;

            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Orders");
            await xmlWriter.WriteAttributeStringAsync("Version", TvProgVersion.CURRENT_VERSION);

            foreach (var order in orders)
            {
                await xmlWriter.WriteStartElementAsync("Order");

                await xmlWriter.WriteStringAsync("OrderId", order.Id);
                await xmlWriter.WriteStringAsync("OrderGuid", order.OrderGuid, ignore);
                await xmlWriter.WriteStringAsync("StoreId", order.StoreId);
                await xmlWriter.WriteStringAsync("UserId", order.UserId, ignore);
                await xmlWriter.WriteStringAsync("OrderStatusId", order.OrderStatusId, ignore);
                await xmlWriter.WriteStringAsync("PaymentStatusId", order.PaymentStatusId, ignore);
                await xmlWriter.WriteStringAsync("ShippingStatusId", order.ShippingStatusId, ignore);
                await xmlWriter.WriteStringAsync("UserLanguageId", order.UserLanguageId, ignore);
                await xmlWriter.WriteStringAsync("UserTaxDisplayTypeId", order.UserTaxDisplayTypeId, ignore);
                await xmlWriter.WriteStringAsync("UserIp", order.UserIp, ignore);
                await xmlWriter.WriteStringAsync("OrderSubtotalInclTax", order.OrderSubtotalInclTax, ignore);
                await xmlWriter.WriteStringAsync("OrderSubtotalExclTax", order.OrderSubtotalExclTax, ignore);
                await xmlWriter.WriteStringAsync("OrderSubTotalDiscountInclTax", order.OrderSubTotalDiscountInclTax, ignore);
                await xmlWriter.WriteStringAsync("OrderSubTotalDiscountExclTax", order.OrderSubTotalDiscountExclTax, ignore);
                await xmlWriter.WriteStringAsync("OrderShippingInclTax", order.OrderShippingInclTax, ignore);
                await xmlWriter.WriteStringAsync("OrderShippingExclTax", order.OrderShippingExclTax, ignore);
                await xmlWriter.WriteStringAsync("PaymentMethodAdditionalFeeInclTax", order.PaymentMethodAdditionalFeeInclTax, ignore);
                await xmlWriter.WriteStringAsync("PaymentMethodAdditionalFeeExclTax", order.PaymentMethodAdditionalFeeExclTax, ignore);
                await xmlWriter.WriteStringAsync("TaxRates", order.TaxRates, ignore);
                await xmlWriter.WriteStringAsync("OrderTax", order.OrderTax, ignore);
                await xmlWriter.WriteStringAsync("OrderTotal", order.OrderTotal, ignore);
                await xmlWriter.WriteStringAsync("RefundedAmount", order.RefundedAmount, ignore);
                await xmlWriter.WriteStringAsync("OrderDiscount", order.OrderDiscount, ignore);
                await xmlWriter.WriteStringAsync("CurrencyRate", order.CurrencyRate);
                await xmlWriter.WriteStringAsync("UserCurrencyCode", order.UserCurrencyCode);
                await xmlWriter.WriteStringAsync("AffiliateId", order.AffiliateId, ignore);
                await xmlWriter.WriteStringAsync("AllowStoringCreditCardNumber", order.AllowStoringCreditCardNumber, ignore);
                await xmlWriter.WriteStringAsync("CardType", order.CardType, ignore);
                await xmlWriter.WriteStringAsync("CardName", order.CardName, ignore);
                await xmlWriter.WriteStringAsync("CardNumber", order.CardNumber, ignore);
                await xmlWriter.WriteStringAsync("MaskedCreditCardNumber", order.MaskedCreditCardNumber, ignore);
                await xmlWriter.WriteStringAsync("CardCvv2", order.CardCvv2, ignore);
                await xmlWriter.WriteStringAsync("CardExpirationMonth", order.CardExpirationMonth, ignore);
                await xmlWriter.WriteStringAsync("CardExpirationYear", order.CardExpirationYear, ignore);
                await xmlWriter.WriteStringAsync("PaymentMethodSystemName", order.PaymentMethodSystemName, ignore);
                await xmlWriter.WriteStringAsync("AuthorizationTransactionId", order.AuthorizationTransactionId, ignore);
                await xmlWriter.WriteStringAsync("AuthorizationTransactionCode", order.AuthorizationTransactionCode, ignore);
                await xmlWriter.WriteStringAsync("AuthorizationTransactionResult", order.AuthorizationTransactionResult, ignore);
                await xmlWriter.WriteStringAsync("CaptureTransactionId", order.CaptureTransactionId, ignore);
                await xmlWriter.WriteStringAsync("CaptureTransactionResult", order.CaptureTransactionResult, ignore);
                await xmlWriter.WriteStringAsync("SubscriptionTransactionId", order.SubscriptionTransactionId, ignore);
                await xmlWriter.WriteStringAsync("PaidDateUtc", order.PaidDateUtc == null ? string.Empty : order.PaidDateUtc.Value.ToString(CultureInfo.InvariantCulture), ignore);
                await xmlWriter.WriteStringAsync("ShippingMethod", order.ShippingMethod);
                await xmlWriter.WriteStringAsync("ShippingRateComputationMethodSystemName", order.ShippingRateComputationMethodSystemName, ignore);
                await xmlWriter.WriteStringAsync("CustomValuesXml", order.CustomValuesXml, ignore);
                await xmlWriter.WriteStringAsync("VatNumber", order.VatNumber, ignore);
                await xmlWriter.WriteStringAsync("Deleted", order.Deleted, ignore);
                await xmlWriter.WriteStringAsync("CreatedOnUtc", order.CreatedOnUtc);

                if (_orderSettings.ExportWithTvChannels)
                {
                    //a vendor should have access only to his tvChannels
                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: currentVendor?.Id ?? 0);

                    if (orderItems.Any())
                    {
                        await xmlWriter.WriteStartElementAsync("OrderItems");
                        foreach (var orderItem in orderItems)
                        {
                            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(orderItem.TvChannelId);

                            await xmlWriter.WriteStartElementAsync("OrderItem");
                            await xmlWriter.WriteStringAsync("Id", orderItem.Id);
                            await xmlWriter.WriteStringAsync("OrderItemGuid", orderItem.OrderItemGuid);
                            await xmlWriter.WriteStringAsync("Name", tvChannel.Name);
                            await xmlWriter.WriteStringAsync("Sku", await _tvChannelService.FormatSkuAsync(tvChannel, orderItem.AttributesXml));
                            await xmlWriter.WriteStringAsync("PriceExclTax", orderItem.UnitPriceExclTax);
                            await xmlWriter.WriteStringAsync("PriceInclTax", orderItem.UnitPriceInclTax);
                            await xmlWriter.WriteStringAsync("Quantity", orderItem.Quantity);
                            await xmlWriter.WriteStringAsync("DiscountExclTax", orderItem.DiscountAmountExclTax);
                            await xmlWriter.WriteStringAsync("DiscountInclTax", orderItem.DiscountAmountInclTax);
                            await xmlWriter.WriteStringAsync("TotalExclTax", orderItem.PriceExclTax);
                            await xmlWriter.WriteStringAsync("TotalInclTax", orderItem.PriceInclTax);
                            await xmlWriter.WriteEndElementAsync();
                        }

                        await xmlWriter.WriteEndElementAsync();
                    }
                }

                //shipments
                var shipments = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).OrderBy(x => x.CreatedOnUtc).ToList();
                if (shipments.Any())
                {
                    await xmlWriter.WriteStartElementAsync("Shipments");
                    foreach (var shipment in shipments)
                    {
                        await xmlWriter.WriteStartElementAsync("Shipment");
                        await xmlWriter.WriteElementStringAsync("ShipmentId", null, shipment.Id.ToString());
                        await xmlWriter.WriteElementStringAsync("TrackingNumber", null, shipment.TrackingNumber);
                        await xmlWriter.WriteElementStringAsync("TotalWeight", null, shipment.TotalWeight?.ToString() ?? string.Empty);
                        await xmlWriter.WriteElementStringAsync("ShippedDateUtc", null, shipment.ShippedDateUtc.HasValue ? shipment.ShippedDateUtc.ToString() : string.Empty);
                        await xmlWriter.WriteElementStringAsync("DeliveryDateUtc", null, shipment.DeliveryDateUtc?.ToString() ?? string.Empty);
                        await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, shipment.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            //activity log
            await _userActivityService.InsertActivityAsync("ExportOrders",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportOrders"), orders.Count));

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export orders to XLSX
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<byte[]> ExportOrdersToXlsxAsync(IList<Order> orders)
        {
            //a vendor should have access only to part of order information
            var ignore = await _workContext.GetCurrentVendorAsync() != null;

            //lambda expressions for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
            async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressByIdAsync(o.BillingAddressId);

            //property array
            var properties = new[]
            {
                new PropertyByName<Order, Language>("OrderId", (p, l) => p.Id),
                new PropertyByName<Order, Language>("StoreId", (p, l) => p.StoreId),
                new PropertyByName<Order, Language>("OrderGuid", (p, l) => p.OrderGuid, ignore),
                new PropertyByName<Order, Language>("UserId", (p, l) => p.UserId, ignore),
                new PropertyByName<Order, Language>("UserGuid", async (p, l) => (await _userService.GetUserByIdAsync(p.UserId))?.UserGuid, ignore),
                new PropertyByName<Order, Language>("OrderStatus", (p, l) => p.OrderStatusId, ignore)
                {
                    DropDownElements = await OrderStatus.Pending.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order, Language>("PaymentStatus", (p, l) => p.PaymentStatusId, ignore)
                {
                    DropDownElements = await PaymentStatus.Pending.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order, Language>("ShippingStatus", (p, l) => p.ShippingStatusId, ignore)
                {
                    DropDownElements = await ShippingStatus.ShippingNotRequired.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order, Language>("OrderSubtotalInclTax", (p, l) => p.OrderSubtotalInclTax, ignore),
                new PropertyByName<Order, Language>("OrderSubtotalExclTax", (p, l) => p.OrderSubtotalExclTax, ignore),
                new PropertyByName<Order, Language>("OrderSubTotalDiscountInclTax", (p, l) => p.OrderSubTotalDiscountInclTax, ignore),
                new PropertyByName<Order, Language>("OrderSubTotalDiscountExclTax", (p, l) => p.OrderSubTotalDiscountExclTax, ignore),
                new PropertyByName<Order, Language>("OrderShippingInclTax", (p, l) => p.OrderShippingInclTax, ignore),
                new PropertyByName<Order, Language>("OrderShippingExclTax", (p, l) => p.OrderShippingExclTax, ignore),
                new PropertyByName<Order, Language>("PaymentMethodAdditionalFeeInclTax", (p, l) => p.PaymentMethodAdditionalFeeInclTax, ignore),
                new PropertyByName<Order, Language>("PaymentMethodAdditionalFeeExclTax", (p, l) => p.PaymentMethodAdditionalFeeExclTax, ignore),
                new PropertyByName<Order, Language>("TaxRates", (p, l) => p.TaxRates, ignore),
                new PropertyByName<Order, Language>("OrderTax", (p, l) => p.OrderTax, ignore),
                new PropertyByName<Order, Language>("OrderTotal", (p, l) => p.OrderTotal, ignore),
                new PropertyByName<Order, Language>("RefundedAmount", (p, l) => p.RefundedAmount, ignore),
                new PropertyByName<Order, Language>("OrderDiscount", (p, l) => p.OrderDiscount, ignore),
                new PropertyByName<Order, Language>("CurrencyRate", (p, l) => p.CurrencyRate),
                new PropertyByName<Order, Language>("UserCurrencyCode", (p, l) => p.UserCurrencyCode),
                new PropertyByName<Order, Language>("AffiliateId", (p, l) => p.AffiliateId, ignore),
                new PropertyByName<Order, Language>("PaymentMethodSystemName", (p, l) => p.PaymentMethodSystemName, ignore),
                new PropertyByName<Order, Language>("ShippingPickupInStore", (p, l) => p.PickupInStore, ignore),
                new PropertyByName<Order, Language>("ShippingMethod", (p, l) => p.ShippingMethod),
                new PropertyByName<Order, Language>("ShippingRateComputationMethodSystemName", (p, l) => p.ShippingRateComputationMethodSystemName, ignore),
                new PropertyByName<Order, Language>("CustomValuesXml", (p, l) => p.CustomValuesXml, ignore),
                new PropertyByName<Order, Language>("VatNumber", (p, l) => p.VatNumber, ignore),
                new PropertyByName<Order, Language>("CreatedOnUtc", (p, l) => p.CreatedOnUtc),
                new PropertyByName<Order, Language>("BillingFirstName", async (p, l) => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order, Language>("BillingLastName", async (p, l) => (await orderBillingAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order, Language>("BillingMiddleName", async (p, l) => (await orderBillingAddress(p))?.MiddleName ?? string.Empty),
                new PropertyByName<Order, Language>("BillingEmail", async (p, l) => (await orderBillingAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order, Language>("BillingCompany", async (p, l) => (await orderBillingAddress(p))?.Company ?? string.Empty),
                new PropertyByName<Order, Language>("BillingCountry", async (p, l) => (await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("BillingCountryCode", async (p, l) => (await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)))?.TwoLetterIsoCode, ignore),
                new PropertyByName<Order, Language>("BillingStateProvince", async (p, l) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("BillingStateProvinceAbbreviation", async (p, l) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)))?.Abbreviation, ignore),
                new PropertyByName<Order, Language>("BillingCounty", async (p, l) => (await orderBillingAddress(p))?.County ?? string.Empty),
                new PropertyByName<Order, Language>("BillingCity", async (p, l) => (await orderBillingAddress(p))?.City ?? string.Empty),
                new PropertyByName<Order, Language>("BillingAddress1", async (p, l) => (await orderBillingAddress(p))?.Address1 ?? string.Empty),
                new PropertyByName<Order, Language>("BillingAddress2", async (p, l) => (await orderBillingAddress(p))?.Address2 ?? string.Empty),
                new PropertyByName<Order, Language>("BillingZipPostalCode", async (p, l) => (await orderBillingAddress(p))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order, Language>("BillingPhoneNumber", async (p, l) => (await orderBillingAddress(p))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order, Language>("BillingFaxNumber", async (p, l) => (await orderBillingAddress(p))?.FaxNumber ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingFirstName", async (p, l) => (await orderAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingLastName", async (p, l) => (await orderAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingMiddleName", async (p, l) => (await orderAddress(p))?.MiddleName ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingEmail", async (p, l) => (await orderAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingCompany", async (p, l) => (await orderAddress(p))?.Company ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingCountry", async (p, l) => (await _countryService.GetCountryByAddressAsync(await orderAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingCountryCode", async (p, l) => (await _countryService.GetCountryByAddressAsync(await orderAddress(p)))?.TwoLetterIsoCode, ignore),
                new PropertyByName<Order, Language>("ShippingStateProvince", async (p, l) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingStateProvinceAbbreviation", async (p, l) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)))?.Abbreviation, ignore),
                new PropertyByName<Order, Language>("ShippingCounty", async (p, l) => (await orderAddress(p))?.County ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingCity", async (p, l) => (await orderAddress(p))?.City ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingAddress1", async (p, l) => (await orderAddress(p))?.Address1 ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingAddress2", async (p, l) => (await orderAddress(p))?.Address2 ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingZipPostalCode", async (p, l) => (await orderAddress(p))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingPhoneNumber", async (p, l) => (await orderAddress(p))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingFaxNumber", async (p, l) => (await orderAddress(p))?.FaxNumber ?? string.Empty)
            };

            //activity log
            await _userActivityService.InsertActivityAsync("ExportOrders",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportOrders"), orders.Count));

            return _orderSettings.ExportWithTvChannels
                ? await ExportOrderToXlsxWithTvChannelsAsync(properties, orders)
                : await new PropertyManager<Order, Language>(properties, _catalogSettings).ExportToXlsxAsync(orders);
        }

        /// <summary>
        /// Export user list to XLSX
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<byte[]> ExportUsersToXlsxAsync(IList<User> users)
        {
            var vendors = await _vendorService.GetVendorsByUserIdsAsync(users.Select(c => c.Id).ToArray());

            object getVendor(User user)
            {
                if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                    return user.VendorId;

                return vendors.FirstOrDefault(v => v.Id == user.VendorId)?.Name ?? string.Empty;
            }

            async Task<object> getCountry(User user)
            {
                var countryId = user.CountryId;

                if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                    return countryId;

                var country = await _countryService.GetCountryByIdAsync(countryId);

                return country?.Name ?? string.Empty;
            }

            async Task<object> getStateProvince(User user)
            {
                var stateProvinceId = user.StateProvinceId;

                if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                    return stateProvinceId;

                var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(stateProvinceId);

                return stateProvince?.Name ?? string.Empty;
            }

            //property manager 
            var manager = new PropertyManager<User, Language>(new[]
            {
                new PropertyByName<User, Language>("UserId", (p, l) => p.Id),
                new PropertyByName<User, Language>("UserGuid", (p, l) => p.UserGuid),
                new PropertyByName<User, Language>("Email", (p, l) => p.Email),
                new PropertyByName<User, Language>("Username", (p, l) => p.Username),
                new PropertyByName<User, Language>("IsTaxExempt", (p, l) => p.IsTaxExempt),
                new PropertyByName<User, Language>("AffiliateId", (p, l) => p.AffiliateId),
                new PropertyByName<User, Language>("Vendor",  (p, l) => getVendor(p)),
                new PropertyByName<User, Language>("Active", (p, l) => p.Active),
                new PropertyByName<User, Language>("UserRoles",  async (p, l) =>  string.Join(", ",
                    (await _userService.GetUserRolesAsync(p)).Select(role => _catalogSettings.ExportImportRelatedEntitiesByName ? role.Name : role.Id.ToString()))),
                new PropertyByName<User, Language>("IsGuest", async (p, l) => await _userService.IsGuestAsync(p)),
                new PropertyByName<User, Language>("IsRegistered", async (p, l) => await _userService.IsRegisteredAsync(p)),
                new PropertyByName<User, Language>("IsAdministrator", async (p, l) => await _userService.IsAdminAsync(p)),
                new PropertyByName<User, Language>("IsForumModerator", async (p, l) => await _userService.IsForumModeratorAsync(p)),
                new PropertyByName<User, Language>("IsVendor", async (p, l) => await _userService.IsVendorAsync(p)),
                new PropertyByName<User, Language>("CreatedOnUtc", (p, l) => p.CreatedOnUtc),
                //attributes
                new PropertyByName<User, Language>("FirstName", (p, l) => p.FirstName, !_userSettings.FirstNameEnabled),
                new PropertyByName<User, Language>("LastName", (p, l) => p.LastName, !_userSettings.LastNameEnabled),
                new PropertyByName<User, Language>("MiddleName", (p, l) => p.MiddleName, !_userSettings.MiddleNameEnabled),
                new PropertyByName<User, Language>("BirthDate", (p, l) => p.BirthDate, !_userSettings.BirthDateEnabled),
                new PropertyByName<User, Language>("Gender", (p, l) => p.Gender, !_userSettings.GenderEnabled),
                new PropertyByName<User, Language>("Company", (p, l) => p.Company, !_userSettings.CompanyEnabled),
                new PropertyByName<User, Language>("StreetAddress", (p, l) => p.StreetAddress, !_userSettings.StreetAddressEnabled),
                new PropertyByName<User, Language>("StreetAddress2", (p, l) => p.StreetAddress2, !_userSettings.StreetAddress2Enabled),
                new PropertyByName<User, Language>("ZipPostalCode", (p, l) => p.ZipPostalCode, !_userSettings.ZipPostalCodeEnabled),
                new PropertyByName<User, Language>("City", (p, l) => p.City, !_userSettings.CityEnabled),
                new PropertyByName<User, Language>("County", (p, l) => p.County, !_userSettings.CountyEnabled),
                new PropertyByName<User, Language>("Country",  async (p, l) => await getCountry(p), !_userSettings.CountryEnabled),
                new PropertyByName<User, Language>("StateProvince",  async (p, l) => await getStateProvince(p), !_userSettings.StateProvinceEnabled),
                new PropertyByName<User, Language>("SmartPhone", (p, l) => p.SmartPhone, !_userSettings.SmartPhoneEnabled),
                new PropertyByName<User, Language>("PersonalDataAggree", (p, l) => p.PersonalDataAggree, !_userSettings.AcceptPersonalDataAgreementEnabled),
                new PropertyByName<User, Language>("Fax", (p, l) => p.Fax, !_userSettings.FaxEnabled),
                new PropertyByName<User, Language>("VatNumber", (p, l) => p.VatNumber),
                new PropertyByName<User, Language>("VatNumberStatusId", (p, l) => p.VatNumberStatusId),
                new PropertyByName<User, Language>("VatNumberStatus", (p, l) => p.VatNumberStatusId)
                {
                    DropDownElements = await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<User, Language>("TimeZone", (p, l) => p.GmtZone, !_dateTimeSettings.AllowUsersToSetTimeZone),
                new PropertyByName<User, Language>("AvatarPictureId", async (p, l) => await _genericAttributeService.GetAttributeAsync<int>(p, TvProgUserDefaults.AvatarPictureIdAttribute), !_userSettings.AllowUsersToUploadAvatars),
                new PropertyByName<User, Language>("ForumPostCount", async (p, l) => await _genericAttributeService.GetAttributeAsync<int>(p, TvProgUserDefaults.ForumPostCountAttribute)),
                new PropertyByName<User, Language>("Signature", async (p, l) => await _genericAttributeService.GetAttributeAsync<string>(p, TvProgUserDefaults.SignatureAttribute)),
                new PropertyByName<User, Language>("CustomUserAttributes", async (p, l) => await GetCustomUserAttributesAsync(p)),
            }, _catalogSettings);

            //activity log
            await _userActivityService.InsertActivityAsync("ExportUsers",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportUsers"), users.Count));

            return await manager.ExportToXlsxAsync(users);
        }

        /// <summary>
        /// Export user list to XML
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportUsersToXmlAsync(IList<User> users)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Users");
            await xmlWriter.WriteAttributeStringAsync("Version", TvProgVersion.CURRENT_VERSION);

            foreach (var user in users)
            {
                await xmlWriter.WriteStartElementAsync("User");
                await xmlWriter.WriteElementStringAsync("UserId", null, user.Id.ToString());
                await xmlWriter.WriteElementStringAsync("UserGuid", null, user.UserGuid.ToString());
                await xmlWriter.WriteElementStringAsync("Email", null, user.Email);
                await xmlWriter.WriteElementStringAsync("Username", null, user.Username);

                await xmlWriter.WriteElementStringAsync("IsTaxExempt", null, user.IsTaxExempt.ToString());
                await xmlWriter.WriteElementStringAsync("AffiliateId", null, user.AffiliateId.ToString());
                await xmlWriter.WriteElementStringAsync("VendorId", null, user.VendorId.ToString());
                await xmlWriter.WriteElementStringAsync("Active", null, user.Active.ToString());

                await xmlWriter.WriteElementStringAsync("IsGuest", null, (await _userService.IsGuestAsync(user)).ToString());
                await xmlWriter.WriteElementStringAsync("IsRegistered", null, (await _userService.IsRegisteredAsync(user)).ToString());
                await xmlWriter.WriteElementStringAsync("IsAdministrator", null, (await _userService.IsAdminAsync(user)).ToString());
                await xmlWriter.WriteElementStringAsync("IsForumModerator", null, (await _userService.IsForumModeratorAsync(user)).ToString());
                await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, user.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));

                await xmlWriter.WriteElementStringAsync("LastName", null, user.LastName);
                await xmlWriter.WriteElementStringAsync("FirstName", null, user.FirstName);
                await xmlWriter.WriteElementStringAsync("MiddleName", null, user.MiddleName);
                await xmlWriter.WriteElementStringAsync("BirthDate", null, user.BirthDate.HasValue ? user.BirthDate.Value.ToString(CultureInfo.InvariantCulture) : null);
                await xmlWriter.WriteElementStringAsync("SmartPhone", null, user.SmartPhone);
                await xmlWriter.WriteElementStringAsync("Gender", null, user.Gender);
                await xmlWriter.WriteElementStringAsync("Company", null, user.Company);

                await xmlWriter.WriteElementStringAsync("CountryId", null, user.CountryId.ToString());
                await xmlWriter.WriteElementStringAsync("StreetAddress", null, user.StreetAddress);
                await xmlWriter.WriteElementStringAsync("StreetAddress2", null, user.StreetAddress2);
                await xmlWriter.WriteElementStringAsync("ZipPostalCode", null, user.ZipPostalCode);
                await xmlWriter.WriteElementStringAsync("City", null, user.City);
                await xmlWriter.WriteElementStringAsync("County", null, user.County);
                await xmlWriter.WriteElementStringAsync("StateProvinceId", null, user.StateProvinceId.ToString());
                await xmlWriter.WriteElementStringAsync("SmartPhone", null, user.SmartPhone);
                await xmlWriter.WriteElementStringAsync("Fax", null, user.Fax);
                await xmlWriter.WriteElementStringAsync("VatNumber", null, user.VatNumber);
                await xmlWriter.WriteElementStringAsync("VatNumberStatusId", null, user.VatNumberStatusId.ToString());
                await xmlWriter.WriteElementStringAsync("PersonalDataAggree", null, user.PersonalDataAggree.ToString());
                await xmlWriter.WriteElementStringAsync("GmtZone", null, user.GmtZone);

                foreach (var store in await _storeService.GetAllStoresAsync())
                {
                    var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(user.Email, store.Id);
                    var subscribedToNewsletters = newsletter != null && newsletter.Active;
                    await xmlWriter.WriteElementStringAsync($"Newsletter-in-store-{store.Id}", null, subscribedToNewsletters.ToString());
                }

                await xmlWriter.WriteElementStringAsync("AvatarPictureId", null, (await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute)).ToString());
                await xmlWriter.WriteElementStringAsync("ForumPostCount", null, (await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.ForumPostCountAttribute)).ToString());
                await xmlWriter.WriteElementStringAsync("Signature", null, await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SignatureAttribute));

                if (!string.IsNullOrEmpty(user.CustomUserAttributesXML))
                {
                    var selectedUserAttributes = new StringReader(user.CustomUserAttributesXML);
                    var selectedUserAttributesXmlReader = XmlReader.Create(selectedUserAttributes);
                    await xmlWriter.WriteNodeAsync(selectedUserAttributesXmlReader, false);
                }

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            //activity log
            await _userActivityService.InsertActivityAsync("ExportUsers",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportUsers"), users.Count));

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in TXT (string) format
        /// </returns>
        public virtual async Task<string> ExportNewsletterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions)
        {
            if (subscriptions == null)
                throw new ArgumentNullException(nameof(subscriptions));

            const char separator = ',';
            var sb = new StringBuilder();

            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Email"));
            sb.Append(separator);
            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Active"));
            sb.Append(separator);
            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Store"));
            sb.Append(Environment.NewLine);

            foreach (var subscription in subscriptions)
            {
                sb.Append(subscription.Email);
                sb.Append(separator);
                sb.Append(subscription.Active);
                sb.Append(separator);
                sb.Append(subscription.StoreId);
                sb.Append(Environment.NewLine);
            }

            //activity log
            await _userActivityService.InsertActivityAsync("ExportNewsLetterSubscriptions",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportNewsLetterSubscriptions"), subscriptions.Count));

            return sb.ToString();
        }

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result in TXT (string) format
        /// </returns>
        public virtual async Task<string> ExportStatesToTxtAsync(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));

            const char separator = ',';
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append((await _countryService.GetCountryByIdAsync(state.CountryId)).TwoLetterIsoCode);
                sb.Append(separator);
                sb.Append(state.Name);
                sb.Append(separator);
                sb.Append(state.Abbreviation);
                sb.Append(separator);
                sb.Append(state.Published);
                sb.Append(separator);
                sb.Append(state.DisplayOrder);
                sb.Append(Environment.NewLine); //new line
            }

            //activity log
            await _userActivityService.InsertActivityAsync("ExportStates",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportStates"), states.Count));

            return sb.ToString();
        }

        /// <summary>
        /// Export user info (GDPR request) to XLSX 
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user GDPR info
        /// </returns>
        public virtual async Task<byte[]> ExportUserGdprInfoToXlsxAsync(User user, int storeId)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //lambda expressions for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
            async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressByIdAsync(o.BillingAddressId);

            //user info and user attributes
            var userManager = new PropertyManager<User, Language>(new[]
            {
                new PropertyByName<User, Language>("Email", (p, l) => p.Email),
                new PropertyByName<User, Language>("Username", (p, l) => p.Username, !_userSettings.UsernamesEnabled), 
                //attributes
                new PropertyByName<User, Language>("Last name", (p, l) => p.LastName, !_userSettings.LastNameEnabled),
                new PropertyByName<User, Language>("First name", (p, l) => p.FirstName, !_userSettings.FirstNameEnabled),
                new PropertyByName<User, Language>("Middle name", (p, l) => p.MiddleName, !_userSettings.MiddleNameEnabled),
                new PropertyByName<User, Language>("Gender", (p, l) => p.Gender, !_userSettings.GenderEnabled),
                new PropertyByName<User, Language>("Date of birth", (p, l) => p.BirthDate, !_userSettings.BirthDateEnabled),
                new PropertyByName<User, Language>("Company", (p, l) => p.Company, !_userSettings.CompanyEnabled),
                new PropertyByName<User, Language>("Street address", (p, l) => p.StreetAddress, !_userSettings.StreetAddressEnabled),
                new PropertyByName<User, Language>("Street address 2", (p, l) => p.StreetAddress2, !_userSettings.StreetAddress2Enabled),
                new PropertyByName<User, Language>("Zip / postal code", (p, l) => p.ZipPostalCode, !_userSettings.ZipPostalCodeEnabled),
                new PropertyByName<User, Language>("City", (p, l) => p.City, !_userSettings.CityEnabled),
                new PropertyByName<User, Language>("County", (p, l) => p.County, !_userSettings.CountyEnabled),
                new PropertyByName<User, Language>("Country", async (p, l) => (await _countryService.GetCountryByIdAsync(p.CountryId))?.Name ?? string.Empty, !_userSettings.CountryEnabled),
                new PropertyByName<User, Language>("State province", async (p, l) => (await _stateProvinceService.GetStateProvinceByIdAsync(p.StateProvinceId))?.Name ?? string.Empty, !(_userSettings.StateProvinceEnabled && _userSettings.CountryEnabled)),
                new PropertyByName<User, Language>("Fax", (p, l) => p.Fax, !_userSettings.FaxEnabled),
                new PropertyByName<User, Language>("User attributes",  async (p, l) => await GetCustomUserAttributesAsync(p))
            }, _catalogSettings);

            //user orders
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var orderManager = new PropertyManager<Order, Language>(new[]
            {
                new PropertyByName<Order, Language>("Order Number", (p, l) => p.CustomOrderNumber),
                new PropertyByName<Order, Language>("Order status", async (p, l) => await _localizationService.GetLocalizedEnumAsync(p.OrderStatus)),
                new PropertyByName<Order, Language>("Order total", async (p, l) => await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency(p.OrderTotal, p.CurrencyRate), true, p.UserCurrencyCode, false, currentLanguage.Id)),
                new PropertyByName<Order, Language>("Shipping method", (p, l) => p.ShippingMethod),
                new PropertyByName<Order, Language>("Created on", async (p, l) => (await _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc)).ToString("D")),
                new PropertyByName<Order, Language>("Billing last name", async (p, l) => (await orderBillingAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order, Language>("Billing first name", async (p, l) => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order, Language>("Billing middle name", async (p, l) => (await orderBillingAddress(p))?.MiddleName ?? string.Empty),
                new PropertyByName<Order, Language>("Billing email", async (p, l) => (await orderBillingAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order, Language>("Billing company", async (p, l) => (await orderBillingAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
                new PropertyByName<Order, Language>("Billing country", async (p, l) => await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Order, Language>("Billing state province", async (p, l) => await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Order, Language>("Billing county", async (p, l) => (await orderBillingAddress(p))?.County ?? string.Empty, !_addressSettings.CountyEnabled),
                new PropertyByName<Order, Language>("Billing city", async (p, l) => (await orderBillingAddress(p))?.City ?? string.Empty, !_addressSettings.CityEnabled),
                new PropertyByName<Order, Language>("Billing address 1", async (p, l) => (await orderBillingAddress(p))?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Order, Language>("Billing address 2", async (p, l) => (await orderBillingAddress(p))?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Order, Language>("Billing zip postal code", async (p, l) => (await orderBillingAddress(p))?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Order, Language>("Billing phone number", async (p, l) => (await orderBillingAddress(p))?.PhoneNumber ?? string.Empty, !_addressSettings.SmartPhoneEnabled),
                new PropertyByName<Order, Language>("Billing fax number", async (p, l) => (await orderBillingAddress(p))?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled),
                new PropertyByName<Order, Language>("Shipping last name", async (p, l) => (await orderAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order, Language>("Shipping first name", async (p, l) => (await orderAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order, Language>("Shipping middle name", async (p, l) => (await orderAddress(p))?.MiddleName ?? string.Empty),
                new PropertyByName<Order, Language>("Shipping email", async (p, l) => (await orderAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order, Language>("Shipping company", async (p, l) => (await orderAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
                new PropertyByName<Order, Language>("Shipping country", async (p, l) => await _countryService.GetCountryByAddressAsync(await orderAddress(p)) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Order, Language>("Shipping state province", async (p, l) => await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Order, Language>("Shipping county", async (p, l) => (await orderAddress(p))?.County ?? string.Empty, !_addressSettings.CountyEnabled),
                new PropertyByName<Order, Language>("Shipping city", async (p, l) => (await orderAddress(p))?.City ?? string.Empty, !_addressSettings.CityEnabled),
                new PropertyByName<Order, Language>("Shipping address 1", async (p, l) => (await orderAddress(p))?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Order, Language>("Shipping address 2", async (p, l) => (await orderAddress(p))?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Order, Language>("Shipping zip postal code",
                    async (p, l) => (await orderAddress(p))?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Order, Language>("Shipping phone number", async (p, l) => (await orderAddress(p))?.PhoneNumber ?? string.Empty, !_addressSettings.SmartPhoneEnabled),
                new PropertyByName<Order, Language>("Shipping fax number", async (p, l) => (await orderAddress(p))?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled)
            }, _catalogSettings);

            var orderItemsManager = new PropertyManager<OrderItem, Language>(new[]
            {
                new PropertyByName<OrderItem, Language>("SKU", async (oi, l) => await _tvChannelService.FormatSkuAsync(await _tvChannelService.GetTvChannelByIdAsync(oi.TvChannelId), oi.AttributesXml)),
                new PropertyByName<OrderItem, Language>("Name", async (oi, l) => await _localizationService.GetLocalizedAsync(await _tvChannelService.GetTvChannelByIdAsync(oi.TvChannelId), p => p.Name)),
                new PropertyByName<OrderItem, Language>("Price", async (oi, l) => await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency((await _orderService.GetOrderByIdAsync(oi.OrderId)).UserTaxDisplayType == TaxDisplayType.IncludingTax ? oi.UnitPriceInclTax : oi.UnitPriceExclTax, (await _orderService.GetOrderByIdAsync(oi.OrderId)).CurrencyRate), true, (await _orderService.GetOrderByIdAsync(oi.OrderId)).UserCurrencyCode, false, currentLanguage.Id)),
                new PropertyByName<OrderItem, Language>("Quantity", (oi, l) => oi.Quantity),
                new PropertyByName<OrderItem, Language>("Total", async (oi, l) => await _priceFormatter.FormatPriceAsync((await _orderService.GetOrderByIdAsync(oi.OrderId)).UserTaxDisplayType == TaxDisplayType.IncludingTax ? oi.PriceInclTax : oi.PriceExclTax))
            }, _catalogSettings);

            var orders = await _orderService.SearchOrdersAsync(userId: user.Id);

            //user addresses
            var addressManager = new PropertyManager<Address, Language>(new[]
            {
                new PropertyByName<Address, Language>("Last name", (p, l) => p.LastName),
                new PropertyByName<Address, Language>("First name", (p, l) => p.FirstName),
                new PropertyByName<Address, Language>("Middle name", (p, l) => p.MiddleName),
                new PropertyByName<Address, Language>("Email", (p, l) => p.Email),
                new PropertyByName<Address, Language>("Company", (p, l) => p.Company, !_addressSettings.CompanyEnabled),
                new PropertyByName<Address, Language>("Country", async (p, l) => await _countryService.GetCountryByAddressAsync(p) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Address, Language>("State province", async (p, l) => await _stateProvinceService.GetStateProvinceByAddressAsync(p) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Address, Language>("County", (p, l) => p.County, !_addressSettings.CountyEnabled),
                new PropertyByName<Address, Language>("City", (p, l) => p.City, !_addressSettings.CityEnabled),
                new PropertyByName<Address, Language>("Address 1", (p, l) => p.Address1, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Address, Language>("Address 2", (p, l) => p.Address2, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Address, Language>("Zip / postal code", (p, l) => p.ZipPostalCode, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Address, Language>("SmartPhone number", (p, l) => p.PhoneNumber, !_addressSettings.SmartPhoneEnabled),
                new PropertyByName<Address, Language>("Fax number", (p, l) => p.FaxNumber, !_addressSettings.FaxEnabled),
                new PropertyByName<Address, Language>("Custom attributes", async (p, l) => await _userAttributeFormatter.FormatAttributesAsync(p.CustomAttributes, ";"))
            }, _catalogSettings);

            //user private messages
            var privateMessageManager = new PropertyManager<PrivateMessage, Language>(new[]
            {
                new PropertyByName<PrivateMessage, Language>("From", async (pm, l) => await _userService.GetUserByIdAsync(pm.FromUserId) is User cFrom ? (_userSettings.UsernamesEnabled ? cFrom.Username : cFrom.Email) : string.Empty),
                new PropertyByName<PrivateMessage, Language>("To", async (pm, l) => await _userService.GetUserByIdAsync(pm.ToUserId) is User cTo ? (_userSettings.UsernamesEnabled ? cTo.Username : cTo.Email) : string.Empty),
                new PropertyByName<PrivateMessage, Language>("Subject", (pm, l) => pm.Subject),
                new PropertyByName<PrivateMessage, Language>("Text", (pm, l) => pm.Text),
                new PropertyByName<PrivateMessage, Language>("Created on", async (pm, l) => (await _dateTimeHelper.ConvertToUserTimeAsync(pm.CreatedOnUtc, DateTimeKind.Utc)).ToString("D"))
            }, _catalogSettings);

            List<PrivateMessage> pmList = null;
            if (_forumSettings.AllowPrivateMessages)
            {
                pmList = (await _forumService.GetAllPrivateMessagesAsync(storeId, user.Id, 0, null, null, null, null)).ToList();
                pmList.AddRange((await _forumService.GetAllPrivateMessagesAsync(storeId, 0, user.Id, null, null, null, null)).ToList());
            }

            //user GDPR logs
            var gdprLogManager = new PropertyManager<GdprLog, Language>(new[]
            {
                new PropertyByName<GdprLog, Language>("Request type", async (log, l) => await _localizationService.GetLocalizedEnumAsync(log.RequestType)),
                new PropertyByName<GdprLog, Language>("Request details", (log, l) => log.RequestDetails),
                new PropertyByName<GdprLog, Language>("Created on", async (log, l) => (await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc)).ToString("D"))
            }, _catalogSettings);

            var gdprLog = await _gdprService.GetAllLogAsync(user.Id);

            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                // Worksheet names cannot be more than 31 characters
                var userInfoWorksheet = workbook.Worksheets.Add("User info");
                var fWorksheet = workbook.Worksheets.Add("DataForFilters");
                fWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

                //user info and user attributes
                var userInfoRow = 2;
                userManager.CurrentObject = user;
                userManager.WriteDefaultCaption(userInfoWorksheet);
                await userManager.WriteDefaultToXlsxAsync(userInfoWorksheet, userInfoRow);

                //user addresses
                if (await _userService.GetAddressesByUserIdAsync(user.Id) is IList<Address> addresses && addresses.Any())
                {
                    userInfoRow += 2;

                    var cell = userInfoWorksheet.Row(userInfoRow).Cell(1);
                    cell.Value = "Address List";
                    userInfoRow += 1;
                    addressManager.SetCaptionStyle(cell);
                    addressManager.WriteDefaultCaption(userInfoWorksheet, userInfoRow);

                    foreach (var userAddress in addresses)
                    {
                        userInfoRow += 1;
                        addressManager.CurrentObject = userAddress;
                        await addressManager.WriteDefaultToXlsxAsync(userInfoWorksheet, userInfoRow);
                    }
                }

                //user orders
                if (orders.Any())
                {
                    var ordersWorksheet = workbook.Worksheets.Add("Orders");

                    orderManager.WriteDefaultCaption(ordersWorksheet);

                    var orderRow = 1;

                    foreach (var order in orders)
                    {
                        orderRow += 1;
                        orderManager.CurrentObject = order;
                        await orderManager.WriteDefaultToXlsxAsync(ordersWorksheet, orderRow);

                        //tvChannels
                        var orederItems = await _orderService.GetOrderItemsAsync(order.Id);

                        if (!orederItems.Any())
                            continue;

                        orderRow += 1;

                        orderItemsManager.WriteDefaultCaption(ordersWorksheet, orderRow, 2);
                        ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                        ordersWorksheet.Row(orderRow).Collapse();

                        foreach (var orederItem in orederItems)
                        {
                            orderRow++;
                            orderItemsManager.CurrentObject = orederItem;
                            await orderItemsManager.WriteDefaultToXlsxAsync(ordersWorksheet, orderRow, 2, fWorksheet);
                            ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                            ordersWorksheet.Row(orderRow).Collapse();
                        }
                    }
                }

                //user private messages
                if (pmList?.Any() ?? false)
                {
                    var privateMessageWorksheet = workbook.Worksheets.Add("Private messages");
                    privateMessageManager.WriteDefaultCaption(privateMessageWorksheet);

                    var privateMessageRow = 1;

                    foreach (var privateMessage in pmList)
                    {
                        privateMessageRow += 1;

                        privateMessageManager.CurrentObject = privateMessage;
                        await privateMessageManager.WriteDefaultToXlsxAsync(privateMessageWorksheet, privateMessageRow);
                    }
                }

                //user GDPR logs
                if (gdprLog.Any())
                {
                    var gdprLogWorksheet = workbook.Worksheets.Add("GDPR requests (log)");
                    gdprLogManager.WriteDefaultCaption(gdprLogWorksheet);

                    var gdprLogRow = 1;

                    foreach (var log in gdprLog)
                    {
                        gdprLogRow += 1;

                        gdprLogManager.CurrentObject = log;
                        await gdprLogManager.WriteDefaultToXlsxAsync(gdprLogWorksheet, gdprLogRow);
                    }
                }

                workbook.SaveAs(stream);
            }

            return stream.ToArray();
        }

        #endregion
    }
}
