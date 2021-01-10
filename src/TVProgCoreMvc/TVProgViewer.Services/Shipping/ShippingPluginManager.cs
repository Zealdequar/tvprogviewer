using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Shipping
{
    /// <summary>
    /// Represents a shipping plugin manager implementation
    /// </summary>
    public partial class ShippingPluginManager : PluginManager<IShippingRateComputationMethod>, IShippingPluginManager
    {
        #region Fields

        private readonly ShippingSettings _shippingSettings;

        #endregion

        #region Ctor

        public ShippingPluginManager(IPluginService pluginService,
            ShippingSettings shippingSettings) : base(pluginService)
        {
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active shipping providers
        /// </summary>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="systemName">Filter by shipping provider system name; pass null to load all plugins</param>
        /// <returns>List of active shipping providers</returns>
        public virtual IList<IShippingRateComputationMethod> LoadActivePlugins(User User = null, int storeId = 0, string systemName = null)
        {
            var shippingProviders = LoadActivePlugins(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, User, storeId);

            //filter by passed system name
            if (!string.IsNullOrEmpty(systemName))
            {
                shippingProviders = shippingProviders
                    .Where(provider => provider.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            return shippingProviders;
        }

        /// <summary>
        /// Check whether the passed shipping provider is active
        /// </summary>
        /// <param name="shippingProvider">Shipping provider to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IShippingRateComputationMethod shippingProvider)
        {
            return IsPluginActive(shippingProvider, _shippingSettings.ActiveShippingRateComputationMethodSystemNames);
        }

        /// <summary>
        /// Check whether the shipping provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of shipping provider to check</param>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(string systemName, User User = null, int storeId = 0)
        {
            var shippingProvider = LoadPluginBySystemName(systemName, User, storeId);
            return IsPluginActive(shippingProvider);
        }

        #endregion
    }
}