﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.WebUI.Areas.Admin.Models.Topics;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the topic model factory
    /// </summary>
    public partial interface ITopicModelFactory
    {
        /// <summary>
        /// Prepare topic search model
        /// </summary>
        /// <param name="searchModel">Topic search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic search model
        /// </returns>
        Task<TopicSearchModel> PrepareTopicSearchModelAsync(TopicSearchModel searchModel);

        /// <summary>
        /// Prepare paged topic list model
        /// </summary>
        /// <param name="searchModel">Topic search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic list model
        /// </returns>
        Task<TopicListModel> PrepareTopicListModelAsync(TopicSearchModel searchModel);

        /// <summary>
        /// Prepare topic model
        /// </summary>
        /// <param name="model">Topic model</param>
        /// <param name="topic">Topic</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic model
        /// </returns>
        Task<TopicModel> PrepareTopicModelAsync(TopicModel model, Topic topic, bool excludeProperties = false);
    }
}