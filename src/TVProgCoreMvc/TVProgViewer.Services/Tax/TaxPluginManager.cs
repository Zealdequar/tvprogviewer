using System.Collections.Generic;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.Services.Tax
{
    /// <summary>
    /// Represents a tax plugin manager implementation
    /// </summary>
    public partial class TaxPluginManager : PluginManager<ITaxProvider>, ITaxPluginManager
    {
        #region Fields

        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public TaxPluginManager(IPluginService pluginService,
            TaxSettings taxSettings) : base(pluginService)
        {
            _taxSettings = taxSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load primary active tax provider
        /// </summary>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Tax provider</returns>
        public virtual ITaxProvider LoadPrimaryPlugin(User User = null, int storeId = 0)
        {
            return LoadPrimaryPlugin(_taxSettings.ActiveTaxProviderSystemName, User, storeId);
        }

        /// <summary>
        /// Check whether the passed tax provider is active
        /// </summary>
        /// <param name="taxProvider">Tax provider to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(ITaxProvider taxProvider)
        {
            return IsPluginActive(taxProvider, new List<string> { _taxSettings.ActiveTaxProviderSystemName });
        }

        /// <summary>
        /// Check whether the tax provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of tax provider to check</param>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(string systemName, User User = null, int storeId = 0)
        {
            var taxProvider = LoadPluginBySystemName(systemName, User, storeId);
            return IsPluginActive(taxProvider);
        }

        #endregion
    }
}