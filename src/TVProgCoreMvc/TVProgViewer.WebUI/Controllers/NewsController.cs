using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Core.Rss;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.News;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Seo;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework;
using TVProgViewer.Web.Framework.Controllers;
using TVProgViewer.Web.Framework.Mvc;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Web.Framework.Security;
using TVProgViewer.WebUI.Models.News;
using TVProgViewer.Core.Events;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Controllers
{
    public partial class NewsController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly INewsModelFactory _newsModelFactory;
        private readonly INewsService _newsService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly NewsSettings _newsSettings;

        #endregion

        #region Ctor

        public NewsController(CaptchaSettings captchaSettings,
            IUserActivityService userActivityService,
            IUserService userService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INewsModelFactory newsModelFactory,
            INewsService newsService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            NewsSettings newsSettings)
        {
            _captchaSettings = captchaSettings;
            _userActivityService = userActivityService;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _newsModelFactory = newsModelFactory;
            _newsService = newsService;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _newsSettings = newsSettings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List(NewsPagingFilteringModel command)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = await _newsModelFactory.PrepareNewsItemListModelAsync(command);
            return View(model);
        }

        public virtual async Task<IActionResult> ListRss(int languageId)
        {
            var feed = new RssFeed(
                $"{await _localizationService.GetLocalizedAsync(await _storeContext.GetCurrentStoreAsync(), x => x.Name)}: News",
                "News",
                new Uri(_webHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!_newsSettings.Enabled)
                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();
            var newsItems = await _newsService.GetAllNewsAsync(languageId, (await _storeContext.GetCurrentStoreAsync()).Id);
            foreach (var n in newsItems)
            {
                var newsUrl = Url.RouteUrl("NewsItem", new { SeName = await _urlRecordService.GetSeNameAsync(n, n.LanguageId, ensureTwoPublishedLanguages: false) }, _webHelper.GetCurrentRequestProtocol());
                items.Add(new RssItem(n.Title, n.Short, new Uri(newsUrl), $"urn:store:{(await _storeContext.GetCurrentStoreAsync()).Id}:news:blog:{n.Id}", n.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        public virtual async Task<IActionResult> NewsItem(int newsItemId)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("Homepage");

            var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
            if (newsItem == null)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !newsItem.Published ||
                //availability dates
                !_newsService.IsNewsAvailable(newsItem) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(newsItem);
            //Check whether the current user has a "Manage news" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            var model = new NewsItemModel();
            model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

            //display "edit" (manage) link
            if (hasAdminAccess)
                DisplayEditLink(Url.Action("NewsItemEdit", "News", new { id = newsItem.Id, area = AreaNames.Admin }));

            return View(model);
        }

        [HttpPost, ActionName("NewsItem")]
        [AutoValidateAntiforgeryToken]
        [FormValueRequired("add-comment")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> NewsCommentAdd(int newsItemId, NewsItemModel model, bool captchaValid)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("Homepage");

            var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
            if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (await _userService.IsGuestAsync(await _workContext.GetCurrentUserAsync()) && !_newsSettings.AllowNotRegisteredUsersToLeaveComments)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("News.Comments.OnlyRegisteredUsersLeaveComments"));
            }

            if (ModelState.IsValid)
            {
                var comment = new NewsComment
                {
                    NewsItemId = newsItem.Id,
                    UserId = (await _workContext.GetCurrentUserAsync()).Id,
                    CommentTitle = model.AddNewComment.CommentTitle,
                    CommentText = model.AddNewComment.CommentText,
                    IsApproved = !_newsSettings.NewsCommentsMustBeApproved,
                    StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                    CreatedOnUtc = DateTime.UtcNow,
                };

                await _newsService.InsertNewsCommentAsync(comment);

                //notify a store owner;
                if (_newsSettings.NotifyAboutNewNewsComments)
                    await _workflowMessageService.SendNewsCommentNotificationMessageAsync(comment, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                await _userActivityService.InsertActivityAsync("PublicStore.AddNewsComment",
                    await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddNewsComment"), comment);

                //raise event
                if (comment.IsApproved)
                    await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

                //The text boxes should be cleared after a comment has been posted
                //That' why we reload the page
                TempData["nop.news.addcomment.result"] = comment.IsApproved
                    ? await _localizationService.GetResourceAsync("News.Comments.SuccessfullyAdded")
                    : await _localizationService.GetResourceAsync("News.Comments.SeeAfterApproving");

                return RedirectToRoute("NewsItem", new { SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
            }

            //If we got this far, something failed, redisplay form
            model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);
            return View(model);
        }

        #endregion
    }
}