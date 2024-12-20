﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Tax
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

        public TaxPluginManager(IUserService userService,
            IPluginService pluginService,
            TaxSettings taxSettings) : base(userService, pluginService)
        {
            _taxSettings = taxSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load primary active tax provider
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax provider
        /// </returns>
        public virtual async Task<ITaxProvider> LoadPrimaryPluginAsync(User user = null, int storeId = 0)
        {
            return await LoadPrimaryPluginAsync(_taxSettings.ActiveTaxProviderSystemName, user, storeId);
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
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0)
        {
            var taxProvider = await LoadPluginBySystemNameAsync(systemName, user, storeId);
            return IsPluginActive(taxProvider);
        }

        #endregion
    }
}