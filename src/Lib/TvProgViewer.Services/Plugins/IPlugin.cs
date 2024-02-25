using System.Threading.Tasks;

namespace TvProgViewer.Services.Plugins
{
    /// <summary>
    /// Interface denoting plug-in attributes that are displayed throughout 
    /// the editing interface.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        string GetConfigurationPageUrl();

        /// <summary>
        /// Gets or sets the plugin descriptor
        /// </summary>
        PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InstallAsync();

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UninstallAsync();

        /// <summary>
        /// Update plugin
        /// </summary>
        /// <param name="currentVersion">Current version of plugin</param>
        /// <param name="targetVersion">New version of plugin</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateAsync(string currentVersion, string targetVersion);

        /// <summary>
        /// Prepare plugin to the uninstallation
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PreparePluginToUninstallAsync();
    }
}
