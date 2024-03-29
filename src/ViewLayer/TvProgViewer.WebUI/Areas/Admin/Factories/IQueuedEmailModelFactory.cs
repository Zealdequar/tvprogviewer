﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.WebUI.Areas.Admin.Models.Messages;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the queued email model factory
    /// </summary>
    public partial interface IQueuedEmailModelFactory
    {
        /// <summary>
        /// Prepare queued email search model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the queued email search model
        /// </returns>
        Task<QueuedEmailSearchModel> PrepareQueuedEmailSearchModelAsync(QueuedEmailSearchModel searchModel);

        /// <summary>
        /// Prepare paged queued email list model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the queued email list model
        /// </returns>
        Task<QueuedEmailListModel> PrepareQueuedEmailListModelAsync(QueuedEmailSearchModel searchModel);

        /// <summary>
        /// Prepare queued email model
        /// </summary>
        /// <param name="model">Queued email model</param>
        /// <param name="queuedEmail">Queued email</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the queued email model
        /// </returns>
        Task<QueuedEmailModel> PrepareQueuedEmailModelAsync(QueuedEmailModel model, QueuedEmail queuedEmail, bool excludeProperties = false);
    }
}