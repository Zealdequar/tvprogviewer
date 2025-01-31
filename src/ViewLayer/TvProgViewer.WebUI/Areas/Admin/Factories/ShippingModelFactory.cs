﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Shipping.Pickup;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Directory;
using TvProgViewer.WebUI.Areas.Admin.Models.Shipping;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shipping model factory implementation
    /// </summary>
    public partial class ShippingModelFactory : IShippingModelFactory
    {
        #region Fields

        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IDateRangeService _dateRangeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IPickupPluginManager _pickupPluginManager;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Ctor

        public ShippingModelFactory(IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            ICountryService countryService,
            IDateRangeService dateRangeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IPickupPluginManager pickupPluginManager,
            IShippingPluginManager shippingPluginManager,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService)
        {
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _countryService = countryService;
            _dateRangeService = dateRangeService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _pickupPluginManager = pickupPluginManager;
            _shippingPluginManager = shippingPluginManager;
            _shippingService = shippingService;
            _stateProvinceService = stateProvinceService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare delivery date search model
        /// </summary>
        /// <param name="searchModel">Delivery date search model</param>
        /// <returns>Delivery date search model</returns>
        protected virtual DeliveryDateSearchModel PrepareDeliveryDateSearchModel(DeliveryDateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvChannel availability range search model
        /// </summary>
        /// <param name="searchModel">TvChannel availability range search model</param>
        /// <returns>TvChannel availability range search model</returns>
        protected virtual TvChannelAvailabilityRangeSearchModel PrepareTvChannelAvailabilityRangeSearchModel(TvChannelAvailabilityRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare shipping provider search model
        /// </summary>
        /// <param name="searchModel">Shipping provider search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping provider search model
        /// </returns>
        public virtual Task<ShippingProviderSearchModel> PrepareShippingProviderSearchModelAsync(ShippingProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged shipping provider list model
        /// </summary>
        /// <param name="searchModel">Shipping provider search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping provider list model
        /// </returns>
        public virtual async Task<ShippingProviderListModel> PrepareShippingProviderListModelAsync(ShippingProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get shipping providers
            var shippingProviders = (await _shippingPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = await new ShippingProviderListModel().PrepareToGridAsync(searchModel, shippingProviders, () =>
            {
                return shippingProviders.SelectAwait(async provider =>
                {
                    //fill in model values from the entity
                    var shippingProviderModel = provider.ToPluginModel<ShippingProviderModel>();

                    //fill in additional values (not existing in the entity)
                    shippingProviderModel.IsActive = _shippingPluginManager.IsPluginActive(provider);
                    shippingProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();

                    shippingProviderModel.LogoUrl = await _shippingPluginManager.GetPluginLogoUrlAsync(provider);

                    return shippingProviderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare pickup point provider search model
        /// </summary>
        /// <param name="searchModel">Pickup point provider search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the pickup point provider search model
        /// </returns>
        public virtual Task<PickupPointProviderSearchModel> PreparePickupPointProviderSearchModelAsync(PickupPointProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged pickup point provider list model
        /// </summary>
        /// <param name="searchModel">Pickup point provider search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the pickup point provider list model
        /// </returns>
        public virtual async Task<PickupPointProviderListModel> PreparePickupPointProviderListModelAsync(PickupPointProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get pickup point providers
            var pickupPointProviders = (await _pickupPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = await new PickupPointProviderListModel().PrepareToGridAsync(searchModel, pickupPointProviders, () =>
            {
                return pickupPointProviders.SelectAwait(async provider =>
                {
                    //fill in model values from the entity
                    var pickupPointProviderModel = provider.ToPluginModel<PickupPointProviderModel>();

                    //fill in additional values (not existing in the entity)
                    pickupPointProviderModel.IsActive = _pickupPluginManager.IsPluginActive(provider);
                    pickupPointProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();

                    pickupPointProviderModel.LogoUrl = await _pickupPluginManager.GetPluginLogoUrlAsync(provider);

                    return pickupPointProviderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare shipping method search model
        /// </summary>
        /// <param name="searchModel">Shipping method search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping method search model
        /// </returns>
        public virtual Task<ShippingMethodSearchModel> PrepareShippingMethodSearchModelAsync(ShippingMethodSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged shipping method list model
        /// </summary>
        /// <param name="searchModel">Shipping method search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping method list model
        /// </returns>
        public virtual async Task<ShippingMethodListModel> PrepareShippingMethodListModelAsync(ShippingMethodSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get shipping methods
            var shippingMethods = (await _shippingService.GetAllShippingMethodsAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new ShippingMethodListModel().PrepareToGrid(searchModel, shippingMethods, () =>
            {
                return shippingMethods.Select(method => method.ToModel<ShippingMethodModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare shipping method model
        /// </summary>
        /// <param name="model">Shipping method model</param>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping method model
        /// </returns>
        public virtual async Task<ShippingMethodModel> PrepareShippingMethodModelAsync(ShippingMethodModel model,
            ShippingMethod shippingMethod, bool excludeProperties = false)
        {
            Func<ShippingMethodLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (shippingMethod != null)
            {
                //fill in model values from the entity
                model ??= shippingMethod.ToModel<ShippingMethodModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(shippingMethod, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(shippingMethod, entity => entity.Description, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare dates and ranges search model
        /// </summary>
        /// <param name="searchModel">Dates and ranges search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the dates and ranges search model
        /// </returns>
        public virtual Task<DatesRangesSearchModel> PrepareDatesRangesSearchModelAsync(DatesRangesSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare nested search models
            PrepareDeliveryDateSearchModel(searchModel.DeliveryDateSearchModel);
            PrepareTvChannelAvailabilityRangeSearchModel(searchModel.TvChannelAvailabilityRangeSearchModel);

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged delivery date list model
        /// </summary>
        /// <param name="searchModel">Delivery date search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the delivery date list model
        /// </returns>
        public virtual async Task<DeliveryDateListModel> PrepareDeliveryDateListModelAsync(DeliveryDateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get delivery dates
            var deliveryDates = (await _dateRangeService.GetAllDeliveryDatesAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new DeliveryDateListModel().PrepareToGrid(searchModel, deliveryDates, () =>
            {
                //fill in model values from the entity
                return deliveryDates.Select(date => date.ToModel<DeliveryDateModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare delivery date model
        /// </summary>
        /// <param name="model">Delivery date model</param>
        /// <param name="deliveryDate">Delivery date</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the delivery date model
        /// </returns>
        public virtual async Task<DeliveryDateModel> PrepareDeliveryDateModelAsync(DeliveryDateModel model, DeliveryDate deliveryDate, bool excludeProperties = false)
        {
            Func<DeliveryDateLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (deliveryDate != null)
            {
                //fill in model values from the entity
                model ??= deliveryDate.ToModel<DeliveryDateModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(deliveryDate, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel availability range list model
        /// </summary>
        /// <param name="searchModel">TvChannel availability range search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel availability range list model
        /// </returns>
        public virtual async Task<TvChannelAvailabilityRangeListModel> PrepareTvChannelAvailabilityRangeListModelAsync(TvChannelAvailabilityRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tvChannel availability ranges
            var tvChannelAvailabilityRanges = (await _dateRangeService.GetAllTvChannelAvailabilityRangesAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new TvChannelAvailabilityRangeListModel().PrepareToGrid(searchModel, tvChannelAvailabilityRanges, () =>
            {
                //fill in model values from the entity
                return tvChannelAvailabilityRanges.Select(range => range.ToModel<TvChannelAvailabilityRangeModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel availability range model
        /// </summary>
        /// <param name="model">TvChannel availability range model</param>
        /// <param name="tvChannelAvailabilityRange">TvChannel availability range</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel availability range model
        /// </returns>
        public virtual async Task<TvChannelAvailabilityRangeModel> PrepareTvChannelAvailabilityRangeModelAsync(TvChannelAvailabilityRangeModel model,
            TvChannelAvailabilityRange tvChannelAvailabilityRange, bool excludeProperties = false)
        {
            Func<TvChannelAvailabilityRangeLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvChannelAvailabilityRange != null)
            {
                //fill in model values from the entity
                model ??= tvChannelAvailabilityRange.ToModel<TvChannelAvailabilityRangeModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvChannelAvailabilityRange, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare warehouse search model
        /// </summary>
        /// <param name="searchModel">Warehouse search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warehouse search model
        /// </returns>
        public virtual Task<WarehouseSearchModel> PrepareWarehouseSearchModelAsync(WarehouseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged warehouse list model
        /// </summary>
        /// <param name="searchModel">Warehouse search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warehouse list model
        /// </returns>
        public virtual async Task<WarehouseListModel> PrepareWarehouseListModelAsync(WarehouseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get warehouses
            var warehouses = (await _shippingService.GetAllWarehousesAsync(
                name: searchModel.SearchName))
                .ToPagedList(searchModel);

            //prepare list model
            var model = new WarehouseListModel().PrepareToGrid(searchModel, warehouses, () =>
            {
                //fill in model values from the entity
                return warehouses.Select(warehouse => warehouse.ToModel<WarehouseModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare warehouse model
        /// </summary>
        /// <param name="model">Warehouse model</param>
        /// <param name="warehouse">Warehouse</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warehouse model
        /// </returns>
        public virtual async Task<WarehouseModel> PrepareWarehouseModelAsync(WarehouseModel model, Warehouse warehouse, bool excludeProperties = false)
        {
            if (warehouse != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = warehouse.ToModel<WarehouseModel>();
                }
            }

            //prepare address model
            var address = await _addressService.GetAddressByIdAsync(warehouse?.AddressId ?? 0);
            if (!excludeProperties && address != null)
                model.Address = address.ToModel(model.Address);
            await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
            model.Address.CountryRequired = true;
            model.Address.ZipPostalCodeRequired = true;

            return model;
        }

        /// <summary>
        /// Prepare shipping method restriction model
        /// </summary>
        /// <param name="model">Shipping method restriction model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping method restriction model
        /// </returns>
        public virtual async Task<ShippingMethodRestrictionModel> PrepareShippingMethodRestrictionModelAsync(ShippingMethodRestrictionModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
            model.AvailableCountries = await countries.SelectAwait(async country =>
            {
                var countryModel = country.ToModel<CountryModel>();
                countryModel.NumberOfStates = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id))?.Count ?? 0;

                return countryModel;
            }).ToListAsync();

            foreach (var shippingMethod in await _shippingService.GetAllShippingMethodsAsync())
            {
                model.AvailableShippingMethods.Add(shippingMethod.ToModel<ShippingMethodModel>());
                foreach (var country in countries)
                {
                    if (!model.Restricted.ContainsKey(country.Id))
                        model.Restricted[country.Id] = new Dictionary<int, bool>();

                    model.Restricted[country.Id][shippingMethod.Id] = await _shippingService.CountryRestrictionExistsAsync(shippingMethod, country.Id);
                }
            }

            return model;
        }

        #endregion
    }
}