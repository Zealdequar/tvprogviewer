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
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Controllers
{
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

        public virtual async Task<IActionResult> Index(int? pageNumber, string tab)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()))
            {
                return Challenge();
            }

            var model = await _privateMessagesModelFactory.PreparePrivateMessageIndexModelAsync(pageNumber, tab);
            return View(model);
        }

        [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
        [AutoValidateAntiforgeryToken]
        public virtual async Task<IActionResult> DeleteInboxPM(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToUserId == (await _workContext.GetCurrentUserAsync()).Id)
                            {
                                pm.IsDeletedByRecipient = true;
                                await _forumService.UpdatePrivateMessageAsync(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        [HttpPost, FormValueRequired("mark-unread"), ActionName("InboxUpdate")]
        [AutoValidateAntiforgeryToken]
        public virtual async Task<IActionResult> MarkUnread(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToUserId == (await _workContext.GetCurrentUserAsync()).Id)
                            {
                                pm.IsRead = false;
                                await _forumService.UpdatePrivateMessageAsync(pm);
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
        public virtual async Task<IActionResult> DeleteSentPM(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("si", "").Trim();
                    if (int.TryParse(id, out var privateMessageId))
                    {
                        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.FromUserId == (await _workContext.GetCurrentUserAsync()).Id)
                            {
                                pm.IsDeletedByAuthor = true;
                                await _forumService.UpdatePrivateMessageAsync(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages", new { tab = "sent" });
        }

        public virtual async Task<IActionResult> SendPM(int toUserId, int? replyToMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
                return RedirectToRoute("Homepage");

            if (await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            var userTo = await _userService.GetUserByIdAsync(toUserId);
            if (userTo == null || await _userService.IsGuestAsync(userTo))
                return RedirectToRoute("PrivateMessages");

            PrivateMessage replyToPM = null;
            if (replyToMessageId.HasValue)
            {
                //reply to a previous PM
                replyToPM = await _forumService.GetPrivateMessageByIdAsync(replyToMessageId.Value);
            }

            var model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(userTo, replyToPM);
            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public virtual async Task<IActionResult> SendPM(SendPrivateMessageModel model)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()))
            {
                return Challenge();
            }

            User toUser;
            var replyToPM = await _forumService.GetPrivateMessageByIdAsync(model.ReplyToMessageId);
            if (replyToPM != null)
            {
                //reply to a previous PM
                if (replyToPM.ToUserId == (await _workContext.GetCurrentUserAsync()).Id || replyToPM.FromUserId == (await _workContext.GetCurrentUserAsync()).Id)
                {
                    //Reply to already sent PM (by current user) should not be sent to yourself
                    toUser = await _userService.GetUserByIdAsync(replyToPM.FromUserId == (await _workContext.GetCurrentUserAsync()).Id
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
                toUser = await _userService.GetUserByIdAsync(model.ToUserId);
            }

            if (toUser == null || await _userService.IsGuestAsync(toUser))
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
                        subject = subject[0.._forumSettings.PMSubjectMaxLength];
                    }

                    var text = model.Message;
                    if (_forumSettings.PMTextMaxLength > 0 && text.Length > _forumSettings.PMTextMaxLength)
                    {
                        text = text[0.._forumSettings.PMTextMaxLength];
                    }

                    var nowUtc = DateTime.UtcNow;

                    var privateMessage = new PrivateMessage
                    {
                        StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                        ToUserId = toUser.Id,
                        FromUserId = (await _workContext.GetCurrentUserAsync()).Id,
                        Subject = subject,
                        Text = text,
                        IsDeletedByAuthor = false,
                        IsDeletedByRecipient = false,
                        IsRead = false,
                        CreatedOnUtc = nowUtc
                    };

                    await _forumService.InsertPrivateMessageAsync(privateMessage);

                    //activity log
                    await _userActivityService.InsertActivityAsync("PublicStore.SendPM",
                        string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.SendPM"), toUser.Email), toUser);

                    return RedirectToRoute("PrivateMessages", new { tab = "sent" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(toUser, replyToPM);
            return View(model);
        }

        public virtual async Task<IActionResult> ViewPM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()))
            {
                return Challenge();
            }

            var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
            if (pm != null)
            {
                if (pm.ToUserId != (await _workContext.GetCurrentUserAsync()).Id && pm.FromUserId != (await _workContext.GetCurrentUserAsync()).Id)
                {
                    return RedirectToRoute("PrivateMessages");
                }

                if (!pm.IsRead && pm.ToUserId == (await _workContext.GetCurrentUserAsync()).Id)
                {
                    pm.IsRead = true;
                    await _forumService.UpdatePrivateMessageAsync(pm);
                }
            }
            else
            {
                return RedirectToRoute("PrivateMessages");
            }

            var model = await _privateMessagesModelFactory.PreparePrivateMessageModelAsync(pm);
            return View(model);
        }

        public virtual async Task<IActionResult> DeletePM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("Homepage");
            }

            if (await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()))
            {
                return Challenge();
            }

            var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
            if (pm != null)
            {
                if (pm.FromUserId == (await _workContext.GetCurrentUserAsync()).Id)
                {
                    pm.IsDeletedByAuthor = true;
                    await _forumService.UpdatePrivateMessageAsync(pm);
                }

                if (pm.ToUserId == (await _workContext.GetCurrentUserAsync()).Id)
                {
                    pm.IsDeletedByRecipient = true;
                    await _forumService.UpdatePrivateMessageAsync(pm);
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        #endregion
    }
}