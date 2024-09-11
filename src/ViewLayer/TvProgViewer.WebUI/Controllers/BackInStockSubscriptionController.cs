using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Seo;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.WebUI.Models.Catalog;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class BackInStockSubscriptionController : BasePublicController
    {
        #region Fields

        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly CatalogSettings _catalogSettings;
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public BackInStockSubscriptionController(CatalogSettings catalogSettings,
            UserSettings userSettings,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IUserService userService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ITvChannelService tvChannelService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _userSettings = userSettings;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _userService = userService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _tvChannelService = tvChannelService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        // TvChannel details page > back in stock subscribe
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> SubscribePopup(int tvChannelId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null || tvChannel.Deleted)
                throw new ArgumentException("No tvChannel found with the specified id");

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var model = new BackInStockSubscribeModel
            {
                TvChannelId = tvChannel.Id,
                TvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name),
                TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvChannel),
                IsCurrentUserRegistered = await _userService.IsRegisteredAsync(user),
                MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions,
                CurrentNumberOfBackInStockSubscriptions = (await _backInStockSubscriptionService
                .GetAllSubscriptionsByUserIdAsync(user.Id, store.Id, 0, 1))
                .TotalCount
            };
            if (tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                tvChannel.BackorderMode == BackorderMode.NoBackorders &&
                tvChannel.AllowBackInStockSubscriptions &&
                await _tvChannelService.GetTotalStockQuantityAsync(tvChannel) <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = await _backInStockSubscriptionService
                    .FindSubscriptionAsync(user.Id, tvChannel.Id, store.Id) != null;
            }

            return PartialView(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubscribePopupPOST(int tvChannelId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null || tvChannel.Deleted)
                throw new ArgumentException("No tvChannel found with the specified id");

            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Content(await _localizationService.GetResourceAsync("BackInStockSubscriptions.OnlyRegistered"));

            if (tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                tvChannel.BackorderMode == BackorderMode.NoBackorders &&
                tvChannel.AllowBackInStockSubscriptions &&
                await _tvChannelService.GetTotalStockQuantityAsync(tvChannel) <= 0)
            {
                //out of stock
                var store = await _storeContext.GetCurrentStoreAsync();
                var subscription = await _backInStockSubscriptionService
                    .FindSubscriptionAsync(user.Id, tvChannel.Id, store.Id);
                if (subscription != null)
                {
                    //subscription already exists
                    //unsubscribe
                    await _backInStockSubscriptionService.DeleteSubscriptionAsync(subscription);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("BackInStockSubscriptions.Notification.Unsubscribed"));
                    return new OkResult();
                }

                //subscription does not exist
                //subscribe
                if ((await _backInStockSubscriptionService
                    .GetAllSubscriptionsByUserIdAsync(user.Id, store.Id, 0, 1))
                    .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Json(new
                    {
                        result = string.Format(await _localizationService.GetResourceAsync("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions)
                    });
                }
                subscription = new BackInStockSubscription
                {
                    UserId = user.Id,
                    TvChannelId = tvChannel.Id,
                    StoreId = store.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _backInStockSubscriptionService.InsertSubscriptionAsync(subscription);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("BackInStockSubscriptions.Notification.Subscribed"));
                return new OkResult();
            }

            //subscription not possible
            return Content(await _localizationService.GetResourceAsync("BackInStockSubscriptions.NotAllowed"));
        }

        // My account / Back in stock subscriptions
        public virtual async Task<IActionResult> UserSubscriptions(int? pageNumber)
        {
            if (_userSettings.HideBackInStockSubscriptionsTab)
            {
                return RedirectToRoute("UserInfo");
            }

            var pageIndex = 0;
            if (pageNumber > 0)
            {
                pageIndex = pageNumber.Value - 1;
            }
            var pageSize = 10;

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var list = await _backInStockSubscriptionService.GetAllSubscriptionsByUserIdAsync(user.Id,
                store.Id, pageIndex, pageSize);

            var model = new UserBackInStockSubscriptionsModel();

            foreach (var subscription in list)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(subscription.TvChannelId);

                if (tvChannel != null)
                {
                    var subscriptionModel = new UserBackInStockSubscriptionsModel.BackInStockSubscriptionModel
                    {
                        Id = subscription.Id,
                        TvChannelId = tvChannel.Id,
                        TvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(tvChannel),
                    };
                    model.Subscriptions.Add(subscriptionModel);
                }
            }

            model.PagerModel = new PagerModel(_localizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "UserBackInStockSubscriptions",
                UseRouteLinks = true,
                RouteValues = new BackInStockSubscriptionsRouteValues { PageNumber = pageIndex }
            };

            return View(model);
        }

        [HttpPost, ActionName("UserSubscriptions")]
        public virtual async Task<IActionResult> UserSubscriptionsPOST(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("biss", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("biss", "").Trim();
                    if (int.TryParse(id, out var subscriptionId))
                    {
                        var subscription = await _backInStockSubscriptionService.GetSubscriptionByIdAsync(subscriptionId);
                        var user = await _workContext.GetCurrentUserAsync();
                        if (subscription != null && subscription.UserId == user.Id)
                        {
                            await _backInStockSubscriptionService.DeleteSubscriptionAsync(subscription);
                        }
                    }
                }
            }

            return RedirectToRoute("UserBackInStockSubscriptions");
        }

        #endregion
    }
}