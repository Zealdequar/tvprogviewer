using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Shipping
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

        public ShippingPluginManager(IUserService userService,
            IPluginService pluginService,
            ShippingSettings shippingSettings) : base(userService, pluginService)
        {
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active shipping providers
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="systemName">Filter by shipping provider system name; pass null to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of active shipping providers
        /// </returns>
        public virtual async Task<IList<IShippingRateComputationMethod>> LoadActivePluginsAsync(User user = null,
            int storeId = 0, string systemName = null)
        {
            var shippingProviders = await LoadActivePluginsAsync(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, user, storeId);

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
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0)
        {
            var shippingProvider = await LoadPluginBySystemNameAsync(systemName, user, storeId);
            return IsPluginActive(shippingProvider);
        }

        #endregion
    }
}