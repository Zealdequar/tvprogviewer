﻿using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Logging;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the activity log model factory
    /// </summary>
    public partial interface IActivityLogModelFactory
    {
        /// <summary>
        /// Prepare activity log search model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the activity log search model
        /// </returns>
        Task<ActivityLogSearchModel> PrepareActivityLogSearchModelAsync(ActivityLogSearchModel searchModel);

        /// <summary>
        /// Prepare activity log types search model
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the activity log types search model
        /// </returns>
        Task<ActivityLogTypeSearchModel> PrepareActivityLogTypeSearchModelAsync(ActivityLogTypeSearchModel searchModel);

        /// <summary>
        /// Prepare paged activity log list model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the activity log list model
        /// </returns>
        Task<ActivityLogListModel> PrepareActivityLogListModelAsync(ActivityLogSearchModel searchModel);
    }
}