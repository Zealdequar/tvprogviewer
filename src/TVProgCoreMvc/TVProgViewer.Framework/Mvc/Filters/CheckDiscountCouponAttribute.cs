using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Defaults;
using TVProgViewer.Services.Discounts;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Messages;

namespace TVProgViewer.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that checks and applied discount coupon code to User
    /// </summary>
    public sealed class CheckDiscountCouponAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CheckDiscountCouponAttribute() : base(typeof(CheckDiscountCouponFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks and applied discount coupon code to User
        /// </summary>
        private class CheckDiscountCouponFilter : IActionFilter
        {
            #region Fields

            private readonly IUserService _UserService;
            private readonly IDiscountService _discountService;
            private readonly ILocalizationService _localizationService;
            private readonly INotificationService _notificationService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public CheckDiscountCouponFilter(IUserService UserService,
                IDiscountService discountService,
                ILocalizationService localizationService,
                INotificationService notificationService,
                IWorkContext workContext)
            {
                _UserService = UserService;
                _discountService = discountService;
                _localizationService = localizationService;
                _notificationService = notificationService;
                _workContext = workContext;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //check request query parameters
                if (!context.HttpContext.Request?.Query?.Any() ?? true)
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                var currentUser = _workContext.CurrentUser;

                //ignore search engines
                if (currentUser.IsSearchEngineAccount())
                    return;

                //try to get discount coupon code
                var queryKey = TVProgViewerDiscountDefaults.DiscountCouponQueryParameter;
                if (!context.HttpContext.Request.Query.TryGetValue(queryKey, out var couponCodes) || StringValues.IsNullOrEmpty(couponCodes))
                    return;

                //get validated discounts with passed coupon codes
                var discounts = couponCodes
                    .SelectMany(couponCode => _discountService.GetAllDiscounts(couponCode: couponCode))
                    .Distinct()
                    .ToList();

                var validCouponCodes = new List<string>();

                foreach (var discount in discounts)
                {
                    if (!_discountService.ValidateDiscount(discount, currentUser, couponCodes.ToArray()).IsValid)
                        continue;
                    
                    //apply discount coupon codes to User
                    _UserService.ApplyDiscountCouponCode(currentUser, discount.CouponCode);
                    validCouponCodes.Add(discount.CouponCode);
                }

                //show notifications for activated coupon codes
                foreach (var validCouponCode in validCouponCodes.Distinct())
                {
                    _notificationService.SuccessNotification(
                        string.Format(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.Activated"),
                            validCouponCode));
                }

                //show notifications for invalid coupon codes
                foreach (var invalidCouponCode in couponCodes.Except(
                    validCouponCodes.Distinct()))
                {
                    _notificationService.WarningNotification(
                        string.Format(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.Invalid"),
                            invalidCouponCode));
                }

            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            #endregion
        }

        #endregion
    }
}