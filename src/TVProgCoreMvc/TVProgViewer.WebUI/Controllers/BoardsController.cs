using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Core.Rss;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Forums;
using TVProgViewer.Services.Localization;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Mvc;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Web.Framework.Security;
using TVProgViewer.WebUI.Models.Boards;
using TVProgViewer.WebUI.Controllers;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class BoardsController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IUserService _userService;
        private readonly IForumModelFactory _forumModelFactory;
        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public BoardsController(CaptchaSettings captchaSettings,
            ForumSettings forumSettings,
            IUserService userService,
            IForumModelFactory forumModelFactory,
            IForumService forumService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _captchaSettings = captchaSettings;
            _forumSettings = forumSettings;
            _userService = userService;
            _forumModelFactory = forumModelFactory;
            _forumService = forumService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Index()
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await _forumModelFactory.PrepareBoardsIndexModelAsync();

            return View(model);
        }

        public virtual async Task<IActionResult> ActiveDiscussions(int forumId = 0, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync(forumId, pageNumber);

            return View(model);
        }

        public virtual async Task<IActionResult> ActiveDiscussionsRss(int forumId = 0)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            if (!_forumSettings.ActiveDiscussionsFeedEnabled)
                return RedirectToRoute("Boards");

            var topics = await _forumService.GetActiveTopicsAsync(forumId, 0, _forumSettings.ActiveDiscussionsFeedCount);
            var url = Url.RouteUrl("ActiveDiscussionsRSS", null, _webHelper.GetCurrentRequestProtocol());

            var feedTitle = await _localizationService.GetResourceAsync("Forum.ActiveDiscussionsFeedTitle");
            var feedDescription = await _localizationService.GetResourceAsync("Forum.ActiveDiscussionsFeedDescription");

            var feed = new RssFeed(
                string.Format(feedTitle, await _localizationService.GetLocalizedAsync(await _storeContext.GetCurrentStoreAsync(), x => x.Name)),
                feedDescription,
                new Uri(url),
                DateTime.UtcNow);

            var items = new List<RssItem>();

            var viewsText = await _localizationService.GetResourceAsync("Forum.Views");
            var repliesText = await _localizationService.GetResourceAsync("Forum.Replies");

            foreach (var topic in topics)
            {
                var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = await _forumService.GetTopicSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());
                var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

                items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl),
                    $"urn:store:{(await _storeContext.GetCurrentStoreAsync()).Id}:activeDiscussions:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
            }
            feed.Items = items;

            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        public virtual async Task<IActionResult> ForumGroup(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
            if (forumGroup == null)
                return RedirectToRoute("Boards");

            var model = await _forumModelFactory.PrepareForumGroupModelAsync(forumGroup);

            return View(model);
        }

        public virtual async Task<IActionResult> Forum(int id, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await _forumService.GetForumByIdAsync(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            var model = await _forumModelFactory.PrepareForumPageModelAsync(forum, pageNumber);

            return View(model);
        }

        public virtual async Task<IActionResult> ForumRss(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            if (!_forumSettings.ForumFeedsEnabled)
                return RedirectToRoute("Boards");

            var topicLimit = _forumSettings.ForumFeedCount;
            var forum = await _forumService.GetForumByIdAsync(id);

            if (forum != null)
            {
                //Order by newest topic posts & limit the number of topics to return
                var topics = await _forumService.GetAllTopicsAsync(forum.Id, 0, string.Empty,
                     ForumSearchType.All, 0, 0, topicLimit);

                var url = Url.RouteUrl("ForumRSS", new { id = forum.Id }, _webHelper.GetCurrentRequestProtocol());

                var feedTitle = await _localizationService.GetResourceAsync("Forum.ForumFeedTitle");
                var feedDescription = await _localizationService.GetResourceAsync("Forum.ForumFeedDescription");

                var feed = new RssFeed(
                    string.Format(feedTitle, await _localizationService.GetLocalizedAsync(await _storeContext.GetCurrentStoreAsync(), x => x.Name), forum.Name),
                    feedDescription,
                    new Uri(url),
                    DateTime.UtcNow);

                var items = new List<RssItem>();

                var viewsText = await _localizationService.GetResourceAsync("Forum.Views");
                var repliesText = await _localizationService.GetResourceAsync("Forum.Replies");

                foreach (var topic in topics)
                {
                    var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = await _forumService.GetTopicSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());
                    var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

                    items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl), $"urn:store:{(await _storeContext.GetCurrentStoreAsync()).Id}:forum:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
                }

                feed.Items = items;

                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
            }

            return new RssActionResult(new RssFeed(new Uri(_webHelper.GetStoreLocation())), _webHelper.GetThisPageUrl(false));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> ForumWatch(int id)
        {
            var watchTopic = await _localizationService.GetResourceAsync("Forum.WatchForum");
            var unwatchTopic = await _localizationService.GetResourceAsync("Forum.UnwatchForum");
            var returnText = watchTopic;

            var forum = await _forumService.GetForumByIdAsync(id);
            if (forum == null)
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            if (!await _forumService.IsUserAllowedToSubscribeAsync(await _workContext.GetCurrentUserAsync()))
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var forumSubscription = (await _forumService.GetAllSubscriptionsAsync((await _workContext.GetCurrentUserAsync()).Id,
                forum.Id, 0, 0, 1)).FirstOrDefault();

            bool subscribed;
            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    UserId = (await _workContext.GetCurrentUserAsync()).Id,
                    ForumId = forum.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _forumService.InsertSubscriptionAsync(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                await _forumService.DeleteSubscriptionAsync(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public virtual async Task<IActionResult> Topic(int id, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var model = await _forumModelFactory.PrepareForumTopicPageModelAsync(forumTopic, pageNumber);
            //if no posts loaded, redirect to the first page
            if (!model.ForumPostModels.Any() && pageNumber > 1)
                return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });

            //update view count
            forumTopic.Views += 1;
            await _forumService.UpdateTopicAsync(forumTopic);

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> TopicWatch(int id)
        {
            var watchTopic = await _localizationService.GetResourceAsync("Forum.WatchTopic");
            var unwatchTopic = await _localizationService.GetResourceAsync("Forum.UnwatchTopic");
            var returnText = watchTopic;

            var forumTopic = await _forumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            if (!await _forumService.IsUserAllowedToSubscribeAsync(await _workContext.GetCurrentUserAsync()))
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var forumSubscription = (await _forumService.GetAllSubscriptionsAsync((await _workContext.GetCurrentUserAsync()).Id,
                0, forumTopic.Id, 0, 1)).FirstOrDefault();

            bool subscribed;
            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    UserId = (await _workContext.GetCurrentUserAsync()).Id,
                    TopicId = forumTopic.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _forumService.InsertSubscriptionAsync(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                await _forumService.DeleteSubscriptionAsync(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public virtual async Task<IActionResult> TopicMove(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var model = await _forumModelFactory.PrepareTopicMoveAsync(forumTopic);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicMove(TopicMoveModel model)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicByIdAsync(model.Id);

            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var newForumId = model.ForumSelected;
            var forum = await _forumService.GetForumByIdAsync(newForumId);

            if (forum != null && forumTopic.ForumId != newForumId)
                await _forumService.MoveTopicAsync(forumTopic.Id, newForumId);

            return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage"),
                });

            var forumTopic = await _forumService.GetTopicByIdAsync(id);
            if (forumTopic != null)
            {
                if (!await _forumService.IsUserAllowedToDeleteTopicAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
                    return Challenge();

                var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);

                await _forumService.DeleteTopicAsync(forumTopic);

                if (forum != null)
                    return Json(new
                    {
                        redirect = Url.RouteUrl("ForumSlug", new { id = forum.Id, slug = await _forumService.GetForumSeNameAsync(forum) }),
                    });
            }

            return Json(new
            {
                redirect = Url.RouteUrl("Boards"),
            });
        }

        public virtual async Task<IActionResult> TopicCreate(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await _forumService.GetForumByIdAsync(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            if (await _forumService.IsUserAllowedToCreateTopicAsync(await _workContext.GetCurrentUserAsync(), forum) == false)
                return Challenge();

            var model = new EditForumTopicModel();
            await _forumModelFactory.PrepareTopicCreateModelAsync(forum, model);
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TopicCreate(EditForumTopicModel model, bool captchaValid)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await _forumService.GetForumByIdAsync(model.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _forumService.IsUserAllowedToCreateTopicAsync(await _workContext.GetCurrentUserAsync(), forum))
                    {
                        return Challenge();
                    }

                    var subject = model.Subject;
                    var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject[0..maxSubjectLength];
                    }

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text[0..maxPostLength];

                    var topicType = ForumTopicType.Normal;
                    var ipAddress = _webHelper.GetCurrentIpAddress();
                    var nowUtc = DateTime.UtcNow;

                    if (await _forumService.IsUserAllowedToSetTopicPriorityAsync(await _workContext.GetCurrentUserAsync()))
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                    //forum topic
                    var forumTopic = new ForumTopic
                    {
                        ForumId = forum.Id,
                        UserId = (await _workContext.GetCurrentUserAsync()).Id,
                        TopicTypeId = (int)topicType,
                        Subject = subject,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await _forumService.InsertTopicAsync(forumTopic, true);

                    //forum post
                    var forumPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        UserId = (await _workContext.GetCurrentUserAsync()).Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await _forumService.InsertPostAsync(forumPost, false);

                    //update forum topic
                    forumTopic.NumPosts = 1;
                    forumTopic.LastPostId = forumPost.Id;
                    forumTopic.LastPostUserId = forumPost.UserId;
                    forumTopic.LastPostTime = forumPost.CreatedOnUtc;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    await _forumService.UpdateTopicAsync(forumTopic);

                    //subscription                
                    if (await _forumService.IsUserAllowedToSubscribeAsync(await _workContext.GetCurrentUserAsync()))
                    {
                        if (model.Subscribed)
                        {
                            var forumSubscription = new ForumSubscription
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                UserId = (await _workContext.GetCurrentUserAsync()).Id,
                                TopicId = forumTopic.Id,
                                CreatedOnUtc = nowUtc
                            };

                            await _forumService.InsertSubscriptionAsync(forumSubscription);
                        }
                    }

                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            await _forumModelFactory.PrepareTopicCreateModelAsync(forum, model);

            return View(model);
        }

        public virtual async Task<IActionResult> TopicEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (!await _forumService.IsUserAllowedToEditTopicAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
                return Challenge();

            var model = new EditForumTopicModel();
            await _forumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, false);

            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TopicEdit(EditForumTopicModel model, bool captchaValid)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicByIdAsync(model.Id);

            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _forumService.IsUserAllowedToEditTopicAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
                        return Challenge();

                    var subject = model.Subject;
                    var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject[0..maxSubjectLength];
                    }

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text[0..maxPostLength];

                    var topicType = ForumTopicType.Normal;
                    var ipAddress = _webHelper.GetCurrentIpAddress();
                    var nowUtc = DateTime.UtcNow;

                    if (await _forumService.IsUserAllowedToSetTopicPriorityAsync(await _workContext.GetCurrentUserAsync()))
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                    //forum topic
                    forumTopic.TopicTypeId = (int)topicType;
                    forumTopic.Subject = subject;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    await _forumService.UpdateTopicAsync(forumTopic);

                    //forum post                
                    var firstPost = await _forumService.GetFirstPostAsync(forumTopic);
                    if (firstPost != null)
                    {
                        firstPost.Text = text;
                        firstPost.UpdatedOnUtc = nowUtc;
                        await _forumService.UpdatePostAsync(firstPost);
                    }
                    else
                    {
                        //error (not possible)
                        firstPost = new ForumPost
                        {
                            TopicId = forumTopic.Id,
                            UserId = forumTopic.UserId,
                            Text = text,
                            IPAddress = ipAddress,
                            UpdatedOnUtc = nowUtc
                        };

                        await _forumService.InsertPostAsync(firstPost, false);
                    }

                    //subscription
                    if (await _forumService.IsUserAllowedToSubscribeAsync(await _workContext.GetCurrentUserAsync()))
                    {
                        var forumSubscription = (await _forumService.GetAllSubscriptionsAsync((await _workContext.GetCurrentUserAsync()).Id,
                            0, forumTopic.Id, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = (await _workContext.GetCurrentUserAsync()).Id,
                                    TopicId = forumTopic.Id,
                                    CreatedOnUtc = nowUtc
                                };

                                await _forumService.InsertSubscriptionAsync(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await _forumService.DeleteSubscriptionAsync(forumSubscription);
                            }
                        }
                    }

                    // redirect to the topic page with the topic slug
                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            await _forumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, true);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage"),
                });

            var forumPost = await _forumService.GetPostByIdAsync(id);

            if (forumPost == null)
                return Json(new { redirect = Url.RouteUrl("Boards") });

            if (!await _forumService.IsUserAllowedToDeletePostAsync(await _workContext.GetCurrentUserAsync(), forumPost))
                return Challenge();

            var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
            var forumId = forumTopic.ForumId;
            var forum = await _forumService.GetForumByIdAsync(forumId);
            var forumSlug = await _forumService.GetForumSeNameAsync(forum);

            await _forumService.DeletePostAsync(forumPost);

            //get topic one more time because it can be deleted (first or only post deleted)
            forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
            if (forumTopic == null)
                return Json(new
                {
                    redirect = Url.RouteUrl("ForumSlug", new { id = forumId, slug = forumSlug }),
                });

            return Json(new
            {
                redirect = Url.RouteUrl("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) }),
            });

        }

        public virtual async Task<IActionResult> PostCreate(int id, int? quote)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (!await _forumService.IsUserAllowedToCreatePostAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
                return Challenge();

            var model = await _forumModelFactory.PreparePostCreateModelAsync(forumTopic, quote, false);

            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> PostCreate(EditForumPostModel model, bool captchaValid)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicByIdAsync(model.ForumTopicId);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _forumService.IsUserAllowedToCreatePostAsync(await _workContext.GetCurrentUserAsync(), forumTopic))
                        return Challenge();

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text[0..maxPostLength];

                    var ipAddress = _webHelper.GetCurrentIpAddress();

                    var nowUtc = DateTime.UtcNow;

                    var forumPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        UserId = (await _workContext.GetCurrentUserAsync()).Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await _forumService.InsertPostAsync(forumPost, true);

                    //subscription
                    if (await _forumService.IsUserAllowedToSubscribeAsync(await _workContext.GetCurrentUserAsync()))
                    {
                        var forumSubscription = (await _forumService.GetAllSubscriptionsAsync((await _workContext.GetCurrentUserAsync()).Id,
                            0, forumPost.TopicId, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = (await _workContext.GetCurrentUserAsync()).Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };

                                await _forumService.InsertSubscriptionAsync(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await _forumService.DeleteSubscriptionAsync(forumSubscription);
                            }
                        }
                    }

                    var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;

                    var pageIndex = await _forumService.CalculateTopicPageIndexAsync(forumPost.TopicId, pageSize, forumPost.Id) + 1;
                    string url;
                    if (pageIndex > 1)
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex });
                    else
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
                    return LocalRedirect($"{url}#{forumPost.Id}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            model = await _forumModelFactory.PreparePostCreateModelAsync(forumTopic, 0, true);

            return View(model);
        }

        public virtual async Task<IActionResult> PostEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumPost = await _forumService.GetPostByIdAsync(id);
            if (forumPost == null)
                return RedirectToRoute("Boards");

            if (!await _forumService.IsUserAllowedToEditPostAsync(await _workContext.GetCurrentUserAsync(), forumPost))
                return Challenge();

            var model = await _forumModelFactory.PreparePostEditModelAsync(forumPost, false);

            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> PostEdit(EditForumPostModel model, bool captchaValid)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumPost = await _forumService.GetPostByIdAsync(model.Id);
            if (forumPost == null)
                return RedirectToRoute("Boards");

            if (!await _forumService.IsUserAllowedToEditPostAsync(await _workContext.GetCurrentUserAsync(), forumPost))
                return Challenge();

            var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var nowUtc = DateTime.UtcNow;

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text[0..maxPostLength];
                    }

                    forumPost.UpdatedOnUtc = nowUtc;
                    forumPost.Text = text;
                    await _forumService.UpdatePostAsync(forumPost);

                    //subscription
                    if (await _forumService.IsUserAllowedToSubscribeAsync(await _workContext.GetCurrentUserAsync()))
                    {
                        var forumSubscription = (await _forumService.GetAllSubscriptionsAsync((await _workContext.GetCurrentUserAsync()).Id,
                            0, forumPost.TopicId, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    UserId = (await _workContext.GetCurrentUserAsync()).Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };
                                await _forumService.InsertSubscriptionAsync(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await _forumService.DeleteSubscriptionAsync(forumSubscription);
                            }
                        }
                    }

                    var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;
                    var pageIndex = (await _forumService.CalculateTopicPageIndexAsync(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    string url;
                    if (pageIndex > 1)
                    {
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex });
                    }
                    else
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
                    }
                    return LocalRedirect($"{url}#{forumPost.Id}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            model = await _forumModelFactory.PreparePostEditModelAsync(forumPost, true);

            return View(model);
        }

        public virtual async Task<IActionResult> Search(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await _forumModelFactory.PrepareSearchModelAsync(searchterms, adv, forumId, within, limitDays, pageNumber);

            return View(model);
        }

        public virtual async Task<IActionResult> UserForumSubscriptions(int? pageNumber)
        {
            if (!_forumSettings.AllowUsersToManageSubscriptions)
                return RedirectToRoute("UserInfo");

            var model = await _forumModelFactory.PrepareUserForumSubscriptionsModelAsync(pageNumber);

            return View(model);
        }

        [HttpPost, ActionName("UserForumSubscriptions")]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UserForumSubscriptionsPOST(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("fs", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("fs", "").Trim();
                    if (int.TryParse(id, out var forumSubscriptionId))
                    {
                        var forumSubscription = await _forumService.GetSubscriptionByIdAsync(forumSubscriptionId);
                        if (forumSubscription != null && forumSubscription.UserId == (await _workContext.GetCurrentUserAsync()).Id)
                        {
                            await _forumService.DeleteSubscriptionAsync(forumSubscription);
                        }
                    }
                }
            }

            return RedirectToRoute("UserForumSubscriptions");
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostVote(int postId, bool isUp)
        {
            if (!_forumSettings.AllowPostVoting)
                return new NullJsonResult();

            var forumPost = await _forumService.GetPostByIdAsync(postId);
            if (forumPost == null)
                return new NullJsonResult();

            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Json(new
                {
                    Error = await _localizationService.GetResourceAsync("Forum.Votes.Login"),
                    VoteCount = forumPost.VoteCount
                });

            if ((await _workContext.GetCurrentUserAsync()).Id == forumPost.UserId)
                return Json(new
                {
                    Error = await _localizationService.GetResourceAsync("Forum.Votes.OwnPost"),
                    VoteCount = forumPost.VoteCount
                });

            var forumPostVote = await _forumService.GetPostVoteAsync(postId, await _workContext.GetCurrentUserAsync());
            if (forumPostVote != null)
            {
                if ((forumPostVote.IsUp && isUp) || (!forumPostVote.IsUp && !isUp))
                    return Json(new
                    {
                        Error = await _localizationService.GetResourceAsync("Forum.Votes.AlreadyVoted"),
                        VoteCount = forumPost.VoteCount
                    });

                await _forumService.DeletePostVoteAsync(forumPostVote);
                return Json(new { VoteCount = forumPost.VoteCount });
            }

            if (await _forumService.GetNumberOfPostVotesAsync(await _workContext.GetCurrentUserAsync(), DateTime.UtcNow.AddDays(-1)) >= _forumSettings.MaxVotesPerDay)
                return Json(new
                {
                    Error = string.Format(await _localizationService.GetResourceAsync("Forum.Votes.MaxVotesReached"), _forumSettings.MaxVotesPerDay),
                    VoteCount = forumPost.VoteCount
                });

            await _forumService.InsertPostVoteAsync(new ForumPostVote
            {
                UserId = (await _workContext.GetCurrentUserAsync()).Id,
                ForumPostId = postId,
                IsUp = isUp,
                CreatedOnUtc = DateTime.UtcNow
            });

            return Json(new { VoteCount = forumPost.VoteCount, IsUp = isUp });
        }

        #endregion
    }
}