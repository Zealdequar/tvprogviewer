using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Seo;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Extensions;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the manufacturer model factory implementation
    /// </summary>
    public partial class ManufacturerModelFactory : IManufacturerModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public ManufacturerModelFactory(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            IManufacturerService manufacturerService,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ITvChannelService tvChannelService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _manufacturerService = manufacturerService;
            _discountService = discountService;
            _discountSupportedModelFactory = discountSupportedModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _tvChannelService = tvChannelService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare manufacturer tvChannel search model
        /// </summary>
        /// <param name="searchModel">Manufacturer tvChannel search model</param>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Manufacturer tvChannel search model</returns>
        protected virtual ManufacturerTvChannelSearchModel PrepareManufacturerTvChannelSearchModel(ManufacturerTvChannelSearchModel searchModel,
            Manufacturer manufacturer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            searchModel.ManufacturerId = manufacturer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare manufacturer search model
        /// </summary>
        /// <param name="searchModel">Manufacturer search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer search model
        /// </returns>
        public virtual async Task<ManufacturerSearchModel> PrepareManufacturerSearchModelAsync(ManufacturerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.List.SearchPublished.UnpublishedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged manufacturer list model
        /// </summary>
        /// <param name="searchModel">Manufacturer search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer list model
        /// </returns>
        public virtual async Task<ManufacturerListModel> PrepareManufacturerListModelAsync(ManufacturerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get manufacturers
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true,
                manufacturerName: searchModel.SearchManufacturerName,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1));

            //prepare grid model
            var model = await new ManufacturerListModel().PrepareToGridAsync(searchModel, manufacturers, () =>
            {
                //fill in model values from the entity
                return manufacturers.SelectAwait(async manufacturer =>
                {
                    var manufacturerModel = manufacturer.ToModel<ManufacturerModel>();

                    manufacturerModel.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, 0, true, false);

                    return manufacturerModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="model">Manufacturer model</param>
        /// <param name="manufacturer">Manufacturer</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer model
        /// </returns>
        public virtual async Task<ManufacturerModel> PrepareManufacturerModelAsync(ManufacturerModel model,
            Manufacturer manufacturer, bool excludeProperties = false)
        {
            Func<ManufacturerLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (manufacturer != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = manufacturer.ToModel<ManufacturerModel>();
                    model.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, 0, true, false);
                }

                //prepare nested search model
                PrepareManufacturerTvChannelSearchModel(model.ManufacturerTvChannelSearchModel, manufacturer);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.Description, languageId, false, false);
                    locale.MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, languageId, false, false);
                };
            }

            //set default values for the new model
            if (manufacturer == null)
            {
                model.PageSize = _catalogSettings.DefaultManufacturerPageSize;
                model.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
                model.Published = true;
                model.AllowUsersToSelectPageSize = true;
                model.PriceRangeFiltering = true;
                model.ManuallyPriceRange = true;
                model.PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom;
                model.PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo;
            }

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available manufacturer templates
            await _baseAdminModelFactory.PrepareManufacturerTemplatesAsync(model.AvailableManufacturerTemplates, false);

            //prepare model discounts
            var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers, showHidden: true, isActive: null);
            await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, manufacturer, availableDiscounts, excludeProperties);

            //prepare model user roles
            await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model, manufacturer, excludeProperties);

            //prepare model stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, manufacturer, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged manufacturer tvChannel list model
        /// </summary>
        /// <param name="searchModel">Manufacturer tvChannel search model</param>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer tvChannel list model
        /// </returns>
        public virtual async Task<ManufacturerTvChannelListModel> PrepareManufacturerTvChannelListModelAsync(ManufacturerTvChannelSearchModel searchModel,
            Manufacturer manufacturer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            //get tvChannel manufacturers
            var tvChannelManufacturers = await _manufacturerService.GetTvChannelManufacturersByManufacturerIdAsync(showHidden: true,
                manufacturerId: manufacturer.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new ManufacturerTvChannelListModel().PrepareToGridAsync(searchModel, tvChannelManufacturers, () =>
            {
                return tvChannelManufacturers.SelectAwait(async tvChannelManufacturer =>
                {
                    //fill in model values from the entity
                    var manufacturerTvChannelModel = tvChannelManufacturer.ToModel<ManufacturerTvChannelModel>();

                    //fill in additional values (not existing in the entity)
                    manufacturerTvChannelModel.TvChannelName = (await _tvChannelService.GetTvChannelByIdAsync(tvChannelManufacturer.TvChannelId))?.Name;

                    return manufacturerTvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel search model to add to the manufacturer
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the manufacturer</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel search model to add to the manufacturer
        /// </returns>
        public virtual async Task<AddTvChannelToManufacturerSearchModel> PrepareAddTvChannelToManufacturerSearchModelAsync(AddTvChannelToManufacturerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tvChannel list model to add to the manufacturer
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the manufacturer</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel list model to add to the manufacturer
        /// </returns>
        public virtual async Task<AddTvChannelToManufacturerListModel> PrepareAddTvChannelToManufacturerListModelAsync(AddTvChannelToManufacturerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddTvChannelToManufacturerListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);

                    return tvChannelModel;
                });
            });

            return model;
        }

        #endregion
    }
}