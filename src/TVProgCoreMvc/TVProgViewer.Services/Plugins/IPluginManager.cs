using System.Collections.Generic;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Plugins
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
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>List of plugins</returns>
        IList<TPlugin> LoadAllPlugins(User User = null, int storeId = 0);

        /// <summary>
        /// Load plugin by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Plugin</returns>
        TPlugin LoadPluginBySystemName(string systemName, User User = null, int storeId = 0);

        /// <summary>
        /// Load primary active plugin
        /// </summary>
        /// <param name="systemName">System name of primary active plugin</param>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Plugin</returns>
        TPlugin LoadPrimaryPlugin(string systemName, User User = null, int storeId = 0);

        /// <summary>
        /// Load active plugins
        /// </summary>
        /// <param name="systemNames">System names of active plugins</param>
        /// <param name="User">Filter by User; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>List of active plugins</returns>
        IList<TPlugin> LoadActivePlugins(List<string> systemNames, User User = null, int storeId = 0);

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
        /// <returns>Logo URL</returns>
        string GetPluginLogoUrl(TPlugin plugin);
    }
}