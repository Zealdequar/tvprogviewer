using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Forums;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Controllers;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Web.Framework.Security;
using TVProgViewer.WebUI.Models.PrivateMessages;

namespace TVProgViewer.WebUI.Controllers
{
    [HttpsRequirement(SslRequirement.Yes)]
    public partial class PrivateMessagesController : BasePublicController
    {
        #region Fields

        private readonly ForumSettings _forumSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PrivateMessagesController(ForumSettings forumSettings,
            IUserActivityService userActivityService,
            IUserService userService,
            IForumService forumService,
            ILocalizationService localizationService,
            IPrivateMessagesModelFactory privateMessagesModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _forumSettings = forumSettings;
            _userActivityService = userActivityService;
            _userService = userService;
            _forumService = forumService;
            _localizationService = localizationService;
            _privateMessagesModelFactory = privateMessagesModelFactory;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion
        
        #region Methods

        public virtual IActionResult Index(int? pageNumber, string tab)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (_userService.IsGuest(_workContext.CurrentUser))
            {
                return Challenge();
            }

            var model = _privateMessagesModelFactory.PreparePrivateMessageIndexModel(pageNumber, tab);
            return View(model);
        }
        
        [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
        [AutoValidateAntiforgeryToken]
        public virtual IActionResult DeleteInboxPM(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToUserId == _workContext.CurrentUser.Id)
                            {
                                pm.IsDeletedByRecipient = true;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        [HttpPost, FormValueRequired("mark-unread"), ActionName("InboxUpdate")]
        [AutoValidateAntiforgeryToken]
        public virtual IActionResult MarkUnread(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToUserId == _workContext.CurrentUser.Id)
                            {
                                pm.IsRead = false;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        //updates sent items (deletes PrivateMessages)
        [HttpPost, FormValueRequired("delete-sent"), ActionName("SentUpdate")]
        [AutoValidateAntiforgeryToken]
        public virtual IActionResult DeleteSentPM(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("si", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.FromUserId == _workContext.CurrentUser.Id)
                            {
                                pm.IsDeletedByAuthor = true;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages", new {tab = "sent"});
        }

        public virtual IActionResult SendPM(int toUserId, int? replyToMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
                return RedirectToRoute("Homepage");

            if (_userService.IsGuest(_workContext.CurrentUser))
                return Challenge();

            var userTo = _userService.GetUserById(toUserId);
            if (userTo == null || _userService.IsGuest(userTo))
                return RedirectToRoute("PrivateMessages");

            PrivateMessage replyToPM = null;
            if (replyToMessageId.HasValue)
            {
                //reply to a previous PM
                replyToPM = _forumService.GetPrivateMessageById(replyToMessageId.Value);
            }

            var model = _privateMessagesModelFactory.PrepareSendPrivateMessageModel(userTo, replyToPM);
            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public virtual IActionResult SendPM(SendPrivateMessageModel model)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (_userService.IsGuest(_workContext.CurrentUser))
            {
                return Challenge();
            }

            User toUser;
            var replyToPM = _forumService.GetPrivateMessageById(model.ReplyToMessageId);
            if (replyToPM != null)
            {
                //reply to a previous PM
                if (replyToPM.ToUserId == _workContext.CurrentUser.Id || replyToPM.FromUserId == _workContext.CurrentUser.Id)
                {
                    //Reply to already sent PM (by current user) should not be sent to yourself
                    toUser = _userService.GetUserById(replyToPM.FromUserId == _workContext.CurrentUser.Id
                        ? replyToPM.ToUserId
                        : replyToPM.FromUserId);
                }
                else
                {
                    return RedirectToRoute("PrivateMessages");
                }
            }
            else
            {
                //first PM
                toUser = _userService.GetUserById(model.ToUserId);
            }

            if (toUser == null || _userService.IsGuest(toUser))
            {
                return RedirectToRoute("PrivateMessages");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var subject = model.Subject;
                    if (_forumSettings.PMSubjectMaxLength > 0 && subject.Length > _forumSettings.PMSubjectMaxLength)
                    {
                        subject = subject.Substring(0, _forumSettings.PMSubjectMaxLength);
                    }

                    var text = model.Message;
                    if (_forumSettings.PMTextMaxLength > 0 && text.Length > _forumSettings.PMTextMaxLength)
                    {
                        text = text.Substring(0, _forumSettings.PMTextMaxLength);
                    }

                    var nowUtc = DateTime.UtcNow;

                    var privateMessage = new PrivateMessage
                    {
                        StoreId = _storeContext.CurrentStore.Id,
                        ToUserId = toUser.Id,
                        FromUserId = _workContext.CurrentUser.Id,
                        Subject = subject,
                        Text = text,
                        IsDeletedByAuthor = false,
                        IsDeletedByRecipient = false,
                        IsRead = false,
                        CreatedOnUtc = nowUtc
                    };

                    _forumService.InsertPrivateMessage(privateMessage);

                    //activity log
                    _userActivityService.InsertActivity("PublicStore.SendPM",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.SendPM"), toUser.Email), toUser);

                    return RedirectToRoute("PrivateMessages", new { tab = "sent" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model = _privateMessagesModelFactory.PrepareSendPrivateMessageModel(toUser, replyToPM);
            return View(model);
        }

        public virtual IActionResult ViewPM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (_userService.IsGuest(_workContext.CurrentUser))
            {
                return Challenge();
            }

            var pm = _forumService.GetPrivateMessageById(privateMessageId);
            if (pm != null)
            {
                if (pm.ToUserId != _workContext.CurrentUser.Id && pm.FromUserId != _workContext.CurrentUser.Id)
                {
                    return RedirectToRoute("PrivateMessages");
                }

                if (!pm.IsRead && pm.ToUserId == _workContext.CurrentUser.Id)
                {
                    pm.IsRead = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }
            else
            {
                return RedirectToRoute("PrivateMessages");
            }

            var model = _privateMessagesModelFactory.PreparePrivateMessageModel(pm);
            return View(model);
        }

        public virtual IActionResult DeletePM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (_userService.IsGuest(_workContext.CurrentUser))
            {
                return Challenge();
            }

            var pm = _forumService.GetPrivateMessageById(privateMessageId);
            if (pm != null)
            {
                if (pm.FromUserId == _workContext.CurrentUser.Id)
                {
                    pm.IsDeletedByAuthor = true;
                    _forumService.UpdatePrivateMessage(pm);
                }

                if (pm.ToUserId == _workContext.CurrentUser.Id)
                {
                    pm.IsDeletedByRecipient = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        #endregion
    }
}