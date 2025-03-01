﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.WebUI.Areas.Admin.Models.Messages;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the message template model factory
    /// </summary>
    public partial interface IMessageTemplateModelFactory
    {
        /// <summary>
        /// Prepare message template search model
        /// </summary>
        /// <param name="searchModel">Message template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the message template search model
        /// </returns>
        Task<MessageTemplateSearchModel> PrepareMessageTemplateSearchModelAsync(MessageTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged message template list model
        /// </summary>
        /// <param name="searchModel">Message template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the message template list model
        /// </returns>
        Task<MessageTemplateListModel> PrepareMessageTemplateListModelAsync(MessageTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare message template model
        /// </summary>
        /// <param name="model">Message template model</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the message template model
        /// </returns>
        Task<MessageTemplateModel> PrepareMessageTemplateModelAsync(MessageTemplateModel model,
            MessageTemplate messageTemplate, bool excludeProperties = false);

        /// <summary>
        /// Prepare test message template model
        /// </summary>
        /// <param name="model">Test message template model</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the st message template model
        /// </returns>
        Task<TestMessageTemplateModel> PrepareTestMessageTemplateModelAsync(TestMessageTemplateModel model,
            MessageTemplate messageTemplate, int languageId);
    }
}