using System;
using System.Collections.Generic;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Forums;
using TVProgViewer.Services.Helpers;
using TVProgViewer.WebUI.Models.Common;
using TVProgViewer.WebUI.Models.PrivateMessages;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the private message model factory
    /// </summary>
    public partial class PrivateMessagesModelFactory : IPrivateMessagesModelFactory
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IForumService _forumService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PrivateMessagesModelFactory(UserSettings userSettings,
            ForumSettings forumSettings,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IForumService forumService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _userSettings = userSettings;
            _forumSettings = forumSettings;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _forumService = forumService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the private message index model
        /// </summary>
        /// <param name="page">Number of items page; pass null to disable paging</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message index model</returns>
        public virtual PrivateMessageIndexModel PreparePrivateMessageIndexModel(int? page, string tab)
        {
            var inboxPage = 0;
            var sentItemsPage = 0;
            var sentItemsTabSelected = false;

            switch (tab)
            {
                case "inbox":
                    if (page.HasValue)
                    {
                        inboxPage = page.Value;
                    }

                    break;
                case "sent":
                    if (page.HasValue)
                    {
                        sentItemsPage = page.Value;
                    }

                    sentItemsTabSelected = true;

                    break;
                default:
                    break;
            }

            var model = new PrivateMessageIndexModel
            {
                InboxPage = inboxPage,
                SentItemsPage = sentItemsPage,
                SentItemsTabSelected = sentItemsTabSelected
            };

            return model;
        }

        /// <summary>
        /// Prepare the inbox model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message list model</returns>
        public virtual PrivateMessageListModel PrepareInboxModel(int page, string tab)
        {
            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.PrivateMessagesPageSize;

            var messages = new List<PrivateMessageModel>();

            var list = _forumService.GetAllPrivateMessages(_storeContext.CurrentStore.Id,
                0, _workContext.CurrentUser.Id, null, null, false, string.Empty, page, pageSize);
            foreach (var pm in list)
                messages.Add(PreparePrivateMessageModel(pm));

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { pageNumber = page, tab = tab }
            };

            var model = new PrivateMessageListModel
            {
                Messages = messages,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the sent model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message list model</returns>
        public virtual PrivateMessageListModel PrepareSentModel(int page, string tab)
        {
            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.PrivateMessagesPageSize;

            var messages = new List<PrivateMessageModel>();

            var list = _forumService.GetAllPrivateMessages(_storeContext.CurrentStore.Id,
                _workContext.CurrentUser.Id, 0, null, false, null, string.Empty, page, pageSize);
            foreach (var pm in list)
                messages.Add(PreparePrivateMessageModel(pm));

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { pageNumber = page, tab = tab }
            };

            var model = new PrivateMessageListModel
            {
                Messages = messages,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the send private message model
        /// </summary>
        /// <param name="userTo">User, recipient of the message</param>
        /// <param name="replyToPM">Private message, pass if reply to a previous message is need</param>
        /// <returns>Send private message model</returns>
        public virtual SendPrivateMessageModel PrepareSendPrivateMessageModel(User userTo, PrivateMessage replyToPM)
        {
            if (userTo == null)
                throw new ArgumentNullException(nameof(userTo));

            var model = new SendPrivateMessageModel
            {
                ToUserId = userTo.Id,
                UserToName = _userService.FormatUsername(userTo),
                AllowViewingToProfile = _userSettings.AllowViewingProfiles && !_userService.IsGuest(userTo)
            };

            if (replyToPM == null)
                return model;

            if (replyToPM.ToUserId == _workContext.CurrentUser.Id ||
                replyToPM.FromUserId == _workContext.CurrentUser.Id)
            {
                model.ReplyToMessageId = replyToPM.Id;
                model.Subject = $"Re: {replyToPM.Subject}";
            }

            return model;
        }

        /// <summary>
        /// Prepare the private message model
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Private message model</returns>
        public virtual PrivateMessageModel PreparePrivateMessageModel(PrivateMessage pm)
        {
            if (pm == null)
                throw new ArgumentNullException(nameof(pm));

            var fromUser = _userService.GetUserById(pm.FromUserId);
            var toUser = _userService.GetUserById(pm.ToUserId);

            var model = new PrivateMessageModel
            {
                Id = pm.Id,
                FromUserId = pm.FromUserId,
                UserFromName = _userService.FormatUsername(fromUser),
                AllowViewingFromProfile = _userSettings.AllowViewingProfiles && !_userService.IsGuest(fromUser),
                ToUserId = pm.ToUserId,
                UserToName = _userService.FormatUsername(toUser),
                AllowViewingToProfile = _userSettings.AllowViewingProfiles && !_userService.IsGuest(toUser),
                Subject = pm.Subject,
                Message = _forumService.FormatPrivateMessageText(pm),
                CreatedOn = _dateTimeHelper.ConvertToUserTime(pm.CreatedOnUtc, DateTimeKind.Utc),
                IsRead = pm.IsRead,
            };

            return model;
        }

        #endregion
    }
}