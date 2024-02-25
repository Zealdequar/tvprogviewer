using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Services.Plugins;
using TvProgViewer.WebUI.Areas.Admin.Models.Plugins;
using TvProgViewer.WebUI.Areas.Admin.Models.Plugins.Marketplace;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the plugin model factory
    /// </summary>
    public partial interface IPluginModelFactory
    {
        /// <summary>
        /// Prepare plugin search model
        /// </summary>
        /// <param name="searchModel">Plugin search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the plugin search model
        /// </returns>
        Task<PluginSearchModel> PreparePluginSearchModelAsync(PluginSearchModel searchModel);

        /// <summary>
        /// Prepare paged plugin list model
        /// </summary>
        /// <param name="searchModel">Plugin search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the plugin list model
        /// </returns>
        Task<PluginListModel> PreparePluginListModelAsync(PluginSearchModel searchModel);

        /// <summary>
        /// Prepare plugin model
        /// </summary>
        /// <param name="model">Plugin model</param>
        /// <param name="pluginDescriptor">Plugin descriptor</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the plugin model
        /// </returns>
        Task<PluginModel> PreparePluginModelAsync(PluginModel model, PluginDescriptor pluginDescriptor, bool excludeProperties = false);

        /// <summary>
        /// Prepare search model of plugins of the official feed
        /// </summary>
        /// <param name="searchModel">Search model of plugins of the official feed</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the search model of plugins of the official feed
        /// </returns>
        Task<OfficialFeedPluginSearchModel> PrepareOfficialFeedPluginSearchModelAsync(OfficialFeedPluginSearchModel searchModel);

        /// <summary>
        /// Prepare paged list model of plugins of the official feed
        /// </summary>
        /// <param name="searchModel">Search model of plugins of the official feed</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list model of plugins of the official feed
        /// </returns>
        Task<OfficialFeedPluginListModel> PrepareOfficialFeedPluginListModelAsync(OfficialFeedPluginSearchModel searchModel);

        /// <summary>
        /// Prepare plugin models for admin navigation
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of models
        /// </returns>
        Task<IList<AdminNavigationPluginModel>> PrepareAdminNavigationPluginModelsAsync();
    }
}