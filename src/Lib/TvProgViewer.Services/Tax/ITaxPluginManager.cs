﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Tax
{
    /// <summary>
    /// Represents a tax plugin manager
    /// </summary>
    public partial interface ITaxPluginManager : IPluginManager<ITaxProvider>
    {
        /// <summary>
        /// Load primary active tax provider
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax provider
        /// </returns>
        Task<ITaxProvider> LoadPrimaryPluginAsync(User user = null, int storeId = 0);

        /// <summary>
        /// Check whether the passed tax provider is active
        /// </summary>
        /// <param name="taxProvider">Tax provider to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(ITaxProvider taxProvider);

        /// <summary>
        /// Check whether the tax provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of tax provider to check</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0);
    }
}