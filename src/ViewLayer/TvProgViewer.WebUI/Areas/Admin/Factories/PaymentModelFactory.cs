﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Payments;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Directory;
using TvProgViewer.WebUI.Areas.Admin.Models.Payments;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the payment method model factory implementation
    /// </summary>
    public partial class PaymentModelFactory : IPaymentModelFactory
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Ctor

        public PaymentModelFactory(ICountryService countryService,
            ILocalizationService localizationService,
            IPaymentPluginManager paymentPluginManager,
            IStateProvinceService stateProvinceService)
        {
            _countryService = countryService;
            _localizationService = localizationService;
            _paymentPluginManager = paymentPluginManager;
            _stateProvinceService = stateProvinceService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare payment methods model
        /// </summary>
        /// <param name="methodsModel">Payment methods model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment methods model
        /// </returns>
        public virtual async Task<PaymentMethodsModel> PreparePaymentMethodsModelAsync(PaymentMethodsModel methodsModel)
        {
            if (methodsModel == null)
                throw new ArgumentNullException(nameof(methodsModel));

            //prepare nested search models
            await PreparePaymentMethodSearchModelAsync(methodsModel.PaymentsMethod);
            await PreparePaymentMethodRestrictionModelAsync(methodsModel.PaymentMethodRestriction);

            return methodsModel;
        }

        /// <summary>
        /// Prepare paged payment method list model
        /// </summary>
        /// <param name="searchModel">Payment method search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment method list model
        /// </returns>
        public virtual async Task<PaymentMethodListModel> PreparePaymentMethodListModelAsync(PaymentMethodSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get payment methods
            var paymentMethods = (await _paymentPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = await new PaymentMethodListModel().PrepareToGridAsync(searchModel, paymentMethods, () =>
            {
                return paymentMethods.SelectAwait(async method =>
                {
                    //fill in model values from the entity
                    var paymentMethodModel = method.ToPluginModel<PaymentMethodModel>();

                    //fill in additional values (not existing in the entity)
                    paymentMethodModel.IsActive = _paymentPluginManager.IsPluginActive(method);
                    paymentMethodModel.ConfigurationUrl = method.GetConfigurationPageUrl();

                    paymentMethodModel.LogoUrl = await _paymentPluginManager.GetPluginLogoUrlAsync(method);
                    paymentMethodModel.RecurringPaymentType = await _localizationService.GetLocalizedEnumAsync(method.RecurringPaymentType);

                    return paymentMethodModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare payment method search model
        /// </summary>
        /// <param name="searchModel">Payment method search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment method search model
        /// </returns>
        public virtual Task<PaymentMethodSearchModel> PreparePaymentMethodSearchModelAsync(PaymentMethodSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare payment method restriction model
        /// </summary>
        /// <param name="model">Payment method restriction model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment method restriction model
        /// </returns>
        public virtual async Task<PaymentMethodRestrictionModel> PreparePaymentMethodRestrictionModelAsync(PaymentMethodRestrictionModel model)
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

            foreach (var method in await _paymentPluginManager.LoadAllPluginsAsync())
            {
                var paymentMethodModel = method.ToPluginModel<PaymentMethodModel>();
                paymentMethodModel.RecurringPaymentType = await _localizationService.GetLocalizedEnumAsync(method.RecurringPaymentType);

                model.AvailablePaymentMethods.Add(paymentMethodModel);

                var restrictedCountries = await _paymentPluginManager.GetRestrictedCountryIdsAsync(method);
                foreach (var country in countries)
                {
                    if (!model.Restricted.ContainsKey(method.PluginDescriptor.SystemName))
                        model.Restricted[method.PluginDescriptor.SystemName] = new Dictionary<int, bool>();

                    model.Restricted[method.PluginDescriptor.SystemName][country.Id] = restrictedCountries.Contains(country.Id);
                }
            }

            return model;
        }

        #endregion
    }
}