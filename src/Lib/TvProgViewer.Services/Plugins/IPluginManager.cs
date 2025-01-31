﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Services.Plugins
{
    /// <summary>
    /// Represents a plugin manager
    /// </summary>
    /// <typeparam name="TPlugin">Type of plugin</typeparam>
    public partial interface IPluginManager<TPlugin> where TPlugin : class, IPlugin
    {
        /// <summary>
        /// Load all plugins
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of plugins
        /// </returns>
        Task<IList<TPlugin>> LoadAllPluginsAsync(User user = null, int storeId = 0);

        /// <summary>
        /// Load plugin by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the plugin
        /// </returns>
        Task<TPlugin> LoadPluginBySystemNameAsync(string systemName, User user = null, int storeId = 0);

        /// <summary>
        /// Load active plugins
        /// </summary>
        /// <param name="systemNames">System names of active plugins</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of active plugins
        /// </returns>
        Task<IList<TPlugin>> LoadActivePluginsAsync(List<string> systemNames, User user = null, int storeId = 0);

        /// <summary>
        /// Check whether the passed plugin is active
        /// </summary>
        /// <param name="plugin">Plugin to check</param>
        /// <param name="systemNames">System names of active plugins</param>
        /// <returns>Result</returns>
        bool IsPluginActive(TPlugin plugin, List<string> systemNames);

        /// <summary>
        /// Get plugin logo URL
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the logo URL
        /// </returns>
        Task<string> GetPluginLogoUrlAsync(TPlugin plugin);
    }
}