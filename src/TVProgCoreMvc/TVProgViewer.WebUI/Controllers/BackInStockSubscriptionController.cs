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
using TVProgViewer.WebUI.Controllers;

namespace TVProgViewer.WebUI.Controllers
{
    public partial class BackInStockSubscriptionController : BasePublicController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly UserSettings _userSettings;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
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
            _productService = productService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        // Product details page > back in stock subscribe
        public virtual IActionResult SubscribePopup(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            var model = new BackInStockSubscribeModel
            {
                ProductId = product.Id,
                ProductName = _localizationService.GetLocalized(product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(product),
                IsCurrentUserRegistered = _userService.IsRegistered(_workContext.CurrentUser),
                MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions,
                CurrentNumberOfBackInStockSubscriptions = _backInStockSubscriptionService
                .GetAllSubscriptionsByUserId(_workContext.CurrentUser.Id, _storeContext.CurrentStore.Id, 0, 1)
                .TotalCount
            };
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                _productService.GetTotalStockQuantity(product) <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentUser.Id, product.Id, _storeContext.CurrentStore.Id) != null;
            }
            return PartialView(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult SubscribePopupPOST(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            if (!_userService.IsRegistered(_workContext.CurrentUser))
                return Content(_localizationService.GetResource("BackInStockSubscriptions.OnlyRegistered"));

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                _productService.GetTotalStockQuantity(product) <= 0)
            {
                //out of stock
                var subscription = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentUser.Id, product.Id, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    //subscription already exists
                    //unsubscribe
                    _backInStockSubscriptionService.DeleteSubscription(subscription);

                    return Json(new
                    {
                        result = "Unsubscribed"
                    });
                }

                //subscription does not exist
                //subscribe
                if (_backInStockSubscriptionService
                    .GetAllSubscriptionsByUserId(_workContext.CurrentUser.Id, _storeContext.CurrentStore.Id, 0, 1)
                    .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Json(new
                    {
                        result = string.Format(_localizationService.GetResource("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions)
                    });
                }
                subscription = new BackInStockSubscription
                {
                    UserId = _workContext.CurrentUser.Id,
                    ProductId = product.Id,
                    StoreId = _storeContext.CurrentStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _backInStockSubscriptionService.InsertSubscription(subscription);

                return Json(new
                {
                    result = "Subscribed"
                });
            }

            //subscription not possible
            return Content(_localizationService.GetResource("BackInStockSubscriptions.NotAllowed"));
        }

        // My account / Back in stock subscriptions
        public virtual IActionResult UserSubscriptions(int? pageNumber)
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

            var user = _workContext.CurrentUser;
            var list = _backInStockSubscriptionService.GetAllSubscriptionsByUserId(user.Id,
                _storeContext.CurrentStore.Id, pageIndex, pageSize);

            var model = new UserBackInStockSubscriptionsModel();

            foreach (var subscription in list)
            {
                var product = _productService.GetProductById(subscription.ProductId);

                if (product != null)
                {
                    var subscriptionModel = new UserBackInStockSubscriptionsModel.BackInStockSubscriptionModel
                    {
                        Id = subscription.Id,
                        ProductId = product.Id,
                        ProductName = _localizationService.GetLocalized(product, x => x.Name),
                        SeName = _urlRecordService.GetSeName(product),
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
        public virtual IActionResult UserSubscriptionsPOST(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("biss", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("biss", "").Trim();
                    if (int.TryParse(id, out var subscriptionId))
                    {
                        var subscription = _backInStockSubscriptionService.GetSubscriptionById(subscriptionId);
                        if (subscription != null && subscription.UserId == _workContext.CurrentUser.Id)
                        {
                            _backInStockSubscriptionService.DeleteSubscription(subscription);
                        }
                    }
                }
            }

            return RedirectToRoute("UserBackInStockSubscriptions");
        }

        #endregion
    }
}