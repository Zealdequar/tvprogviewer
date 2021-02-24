using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Seo;
using TVProgViewer.WebUI.Models.Catalog;
using TVProgViewer.WebUI.Models.Common;
using System.Threading.Tasks;
using TVProgViewer.Services.Messages;

namespace TVProgViewer.WebUI.Controllers
{
    public partial class BackInStockSubscriptionController : BasePublicController
    {
        #region Fields

        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly CatalogSettings _catalogSettings;
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;
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
            IProductService productService,
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
            _productService = productService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        // Product details page > back in stock subscribe
        public virtual async Task<IActionResult> SubscribePopup(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            var model = new BackInStockSubscribeModel
            {
                ProductId = product.Id,
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                IsCurrentUserRegistered = await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()),
                MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions,
                CurrentNumberOfBackInStockSubscriptions = (await _backInStockSubscriptionService
                .GetAllSubscriptionsByUserIdAsync((await _workContext.GetCurrentUserAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id, 0, 1))
                .TotalCount
            };
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await _productService.GetTotalStockQuantityAsync(product) <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = await _backInStockSubscriptionService
                    .FindSubscriptionAsync((await _workContext.GetCurrentUserAsync()).Id, product.Id, (await _storeContext.GetCurrentStoreAsync()).Id) != null;
            }

            return PartialView(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> SubscribePopupPOST(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Content(await _localizationService.GetResourceAsync("BackInStockSubscriptions.OnlyRegistered"));

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await _productService.GetTotalStockQuantityAsync(product) <= 0)
            {
                //out of stock
                var subscription = await _backInStockSubscriptionService
                    .FindSubscriptionAsync((await _workContext.GetCurrentUserAsync()).Id, product.Id, (await _storeContext.GetCurrentStoreAsync()).Id);
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
                    .GetAllSubscriptionsByUserIdAsync((await _workContext.GetCurrentUserAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id, 0, 1))
                    .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Json(new
                    {
                        result = string.Format(await _localizationService.GetResourceAsync("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions)
                    });
                }
                subscription = new BackInStockSubscription
                {
                    UserId = (await _workContext.GetCurrentUserAsync()).Id,
                    ProductId = product.Id,
                    StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
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
            var list = await _backInStockSubscriptionService.GetAllSubscriptionsByUserIdAsync(user.Id,
                (await _storeContext.GetCurrentStoreAsync()).Id, pageIndex, pageSize);

            var model = new UserBackInStockSubscriptionsModel();

            foreach (var subscription in list)
            {
                var product = await _productService.GetProductByIdAsync(subscription.ProductId);

                if (product != null)
                {
                    var subscriptionModel = new UserBackInStockSubscriptionsModel.BackInStockSubscriptionModel
                    {
                        Id = subscription.Id,
                        ProductId = product.Id,
                        ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(product),
                    };
                    model.Subscriptions.Add(subscriptionModel);
                }
            }

            model.PagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "UserBackInStockSubscriptions",
                UseRouteLinks = true,
                RouteValues = new BackInStockSubscriptionsRouteValues { pageNumber = pageIndex }
            };

            return View(model);
        }

        [HttpPost, ActionName("UserSubscriptions")]
        [IgnoreAntiforgeryToken]
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
                        if (subscription != null && subscription.UserId == (await _workContext.GetCurrentUserAsync()).Id)
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