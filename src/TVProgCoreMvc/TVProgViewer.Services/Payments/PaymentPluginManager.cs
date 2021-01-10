using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Payments;
using TVProgViewer.Services.Configuration;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Payments
{
    /// <summary>
    /// Represents a payment plugin manager implementation
    /// </summary>
    public partial class PaymentPluginManager : PluginManager<IPaymentMethod>, IPaymentPluginManager
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly PaymentSettings _paymentSettings;

        #endregion

        #region Ctor

        public PaymentPluginManager(IPluginService pluginService,
            ISettingService settingService,
            PaymentSettings paymentSettings) : base(pluginService)
        {
            _settingService = settingService;
            _paymentSettings = paymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active payment methods
        /// </summary>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="countryId">Filter by country; pass 0 to load all plugins</param>
        /// <returns>List of active payment methods</returns>
        public virtual IList<IPaymentMethod> LoadActivePlugins(User User = null, int storeId = 0, int countryId = 0)
        {
            var paymentMethods = LoadActivePlugins(_paymentSettings.ActivePaymentMethodSystemNames, User, storeId);

            //filter by country
            if (countryId > 0)
                paymentMethods = paymentMethods.Where(method => !GetRestrictedCountryIds(method).Contains(countryId)).ToList();

            return paymentMethods;
        }

        /// <summary>
        /// Check whether the passed payment method is active
        /// </summary>
        /// <param name="paymentMethod">Payment method to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IPaymentMethod paymentMethod)
        {
            return IsPluginActive(paymentMethod, _paymentSettings.ActivePaymentMethodSystemNames);
        }

        /// <summary>
        /// Check whether the payment method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of payment method to check</param>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(string systemName, User User = null, int storeId = 0)
        {
            var paymentMethod = LoadPluginBySystemName(systemName, User, storeId);
            return IsPluginActive(paymentMethod);
        }

        /// <summary>
        /// Get countries in which the passed payment method is now allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>List of country identifiers</returns>
        public virtual IList<int> GetRestrictedCountryIds(IPaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            var settingKey = string.Format(TvProgPaymentDefaults.RestrictedCountriesSettingName, paymentMethod.PluginDescriptor.SystemName);
            return _settingService.GetSettingByKey<List<int>>(settingKey) ?? new List<int>();
        }

        /// <summary>
        /// Save countries in which the passed payment method is now allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="countryIds">List of country identifiers</param>
        public virtual void SaveRestrictedCountries(IPaymentMethod paymentMethod, IList<int> countryIds)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            var settingKey = string.Format(TvProgPaymentDefaults.RestrictedCountriesSettingName, paymentMethod.PluginDescriptor.SystemName);
            _settingService.SetSetting(settingKey, countryIds.ToList());
        }

        #endregion
    }
}