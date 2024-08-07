﻿using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Tasks;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the schedule task model factory
    /// </summary>
    public partial interface IScheduleTaskModelFactory
    {
        /// <summary>
        /// Prepare schedule task search model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the schedule task search model
        /// </returns>
        Task<ScheduleTaskSearchModel> PrepareScheduleTaskSearchModelAsync(ScheduleTaskSearchModel searchModel);

        /// <summary>
        /// Prepare paged schedule task list model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the schedule task list model
        /// </returns>
        Task<ScheduleTaskListModel> PrepareScheduleTaskListModelAsync(ScheduleTaskSearchModel searchModel);
    }
}