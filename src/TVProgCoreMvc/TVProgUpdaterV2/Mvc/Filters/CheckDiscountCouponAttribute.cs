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
using TVProgViewer.Services.Discounts;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Messages;
using System.Threading.Tasks;

namespace TVProgViewer.TVProgUpdaterV2.Mvc.Filters
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
        /// Represents a filter that checks and applied discount coupon code to user
        /// </summary>
        private class CheckDiscountCouponFilter : IAsyncActionFilter
        {
            #region Fields

            private readonly IUserService _userService;
            private readonly IDiscountService _discountService;
            private readonly ILocalizationService _localizationService;
            private readonly INotificationService _notificationService;
            private readonly IWorkContext _workContext;

            #endregion

            #region Ctor

            public CheckDiscountCouponFilter(IUserService userService,
                IDiscountService discountService,
                ILocalizationService localizationService,
                INotificationService notificationService,
                IWorkContext workContext)
            {
                _userService = userService;
                _discountService = discountService;
                _localizationService = localizationService;
                _notificationService = notificationService;
                _workContext = workContext;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that on completion indicates the necessary filter actions have been executed</returns>
            private async Task CheckDiscountCouponAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //check request query parameters
                if (!context.HttpContext.Request?.Query?.Any() ?? true)
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!await DataSettingsManager.IsDatabaseInstalledAsync())
                    return;

                var user = await _workContext.GetCurrentUserAsync();

                //ignore search engines
                if (user.IsSearchEngineAccount())
                    return;

                //try to get discount coupon code
                var queryKey = TvProgDiscountDefaults.DiscountCouponQueryParameter;
                if (!context.HttpContext.Request.Query.TryGetValue(queryKey, out var couponCodes) || StringValues.IsNullOrEmpty(couponCodes))
                    return;

                //get validated discounts with passed coupon codes

                var validCouponCodes = new List<string>();
                var discounts = await couponCodes
                    .SelectManyAwait(async couponCode => await _discountService.GetAllDiscountsAsync(couponCode: couponCode))
                    .Distinct()
                    .ToListAsync();

                foreach (var discount in discounts)
                {
                    var result = await _discountService.ValidateDiscountAsync(discount, user, couponCodes.ToArray());
                    if (!result.IsValid)
                        continue;

                    //apply discount coupon codes to user
                    await _userService.ApplyDiscountCouponCodeAsync(user, discount.CouponCode);

                    validCouponCodes.Add(discount.CouponCode);
                }

                //show notifications for activated coupon codes
                var locale = await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Activated");
                foreach (var validCouponCode in validCouponCodes.Distinct())
                {
                    _notificationService.SuccessNotification(string.Format(locale, validCouponCode));
                }

                //show notifications for invalid coupon codes
                var invalidLocale = await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Invalid");
                foreach (var invalidCouponCode in couponCodes.Except(validCouponCodes.Distinct()))
                {
                    _notificationService.WarningNotification(string.Format(invalidLocale, invalidCouponCode));
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
            /// <returns>A task that on completion indicates the filter has executed</returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                await CheckDiscountCouponAsync(context);
                if (context.Result == null)
                    await next();
            }

            #endregion
        }

        #endregion
    }
}