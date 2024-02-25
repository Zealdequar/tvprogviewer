using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Topics;

namespace TvProgViewer.Services.Topics
{
    /// <summary>
    /// Topic template service interface
    /// </summary>
    public partial interface ITopicTemplateService
    {
        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTopicTemplateAsync(TopicTemplate topicTemplate);

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic templates
        /// </returns>
        Task<IList<TopicTemplate>> GetAllTopicTemplatesAsync();

        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic template
        /// </returns>
        Task<TopicTemplate> GetTopicTemplateByIdAsync(int topicTemplateId);

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTopicTemplateAsync(TopicTemplate topicTemplate);

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTopicTemplateAsync(TopicTemplate topicTemplate);
    }
}
