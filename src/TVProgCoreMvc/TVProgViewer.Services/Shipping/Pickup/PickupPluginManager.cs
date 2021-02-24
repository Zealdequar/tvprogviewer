using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Plugins;
using System.Threading.Tasks;
using TVProgViewer.Services.Users;

namespace TVProgViewer.Services.Shipping.Pickup
{
    /// <summary>
    /// Represents a pickup point plugin manager implementation
    /// </summary>
    public partial class PickupPluginManager : PluginManager<IPickupPointProvider>, IPickupPluginManager
    {
        #region Fields

        private readonly ShippingSettings _shippingSettings;

        #endregion

        #region Ctor

        public PickupPluginManager(IUserService userService,
            IPluginService pluginService,
            ShippingSettings shippingSettings) : base(userService, pluginService)
        {
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active pickup point providers
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="systemName">Filter by pickup point provider system name; pass null to load all plugins</param>
        /// <returns>List of active pickup point providers</returns>
        public virtual async Task<IList<IPickupPointProvider>> LoadActivePluginsAsync(User user = null, int storeId = 0, string systemName = null)
        {
            var pickupPointProviders = await LoadActivePluginsAsync(_shippingSettings.ActivePickupPointProviderSystemNames, user, storeId);

            //filter by passed system name
            if (!string.IsNullOrEmpty(systemName))
            {
                pickupPointProviders = pickupPointProviders
                    .Where(provider => provider.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            return pickupPointProviders;
        }

        /// <summary>
        /// Check whether the passed pickup point provider is active
        /// </summary>
        /// <param name="pickupPointProvider">Pickup point provider to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IPickupPointProvider pickupPointProvider)
        {
            return IsPluginActive(pickupPointProvider, _shippingSettings.ActivePickupPointProviderSystemNames);
        }

        /// <summary>
        /// Check whether the pickup point provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of pickup point provider to check</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual async Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0)
        {
            var pickupPointProvider = await LoadPluginBySystemNameAsync(systemName, user, storeId);
            return IsPluginActive(pickupPointProvider);
        }

        #endregion
    }
}