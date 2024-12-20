﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Cms
{
    /// <summary>
    /// Represents a widget plugin manager
    /// </summary>
    public partial interface IWidgetPluginManager : IPluginManager<IWidgetPlugin>
    {
        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="widgetZone">Widget zone; pass null to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of active widget
        /// </returns>
        Task<IList<IWidgetPlugin>> LoadActivePluginsAsync(User user = null, int storeId = 0, string widgetZone = null);

        /// <summary>
        /// Check whether the passed widget is active
        /// </summary>
        /// <param name="widget">Widget to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(IWidgetPlugin widget);

        /// <summary>
        /// Check whether the widget with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of widget to check</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0);
    }
}