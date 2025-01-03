﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.WebUI.Models.PrivateMessages;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the private message model factory
    /// </summary>
    public partial interface IPrivateMessagesModelFactory
    {
        /// <summary>
        /// Prepare the private message index model
        /// </summary>
        /// <param name="page">Number of items page; pass null to disable paging</param>
        /// <param name="tab">Tab name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the private message index model
        /// </returns>
        Task<PrivateMessageIndexModel> PreparePrivateMessageIndexModelAsync(int? page, string tab);

        /// <summary>
        /// Prepare the inbox model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the private message list model
        /// </returns>
        Task<PrivateMessageListModel> PrepareInboxModelAsync(int page, string tab);

        /// <summary>
        /// Prepare the sent model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the private message list model
        /// </returns>
        Task<PrivateMessageListModel> PrepareSentModelAsync(int page, string tab);

        /// <summary>
        /// Prepare the send private message model
        /// </summary>
        /// <param name="userTo">User, recipient of the message</param>
        /// <param name="replyToPM">Private message, pass if reply to a previous message is need</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the send private message model
        /// </returns>
        Task<SendPrivateMessageModel> PrepareSendPrivateMessageModelAsync(User userTo,
            PrivateMessage replyToPM);

        /// <summary>
        /// Prepare the private message model
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the private message model
        /// </returns>
        Task<PrivateMessageModel> PreparePrivateMessageModelAsync(PrivateMessage pm);
    }
}
