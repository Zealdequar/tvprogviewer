using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Plugin.Tax.Avalara.Services;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Tax;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.Web.Framework.Infrastructure;
using TvProgViewer.WebUI.Models.ShoppingCart;

namespace TvProgViewer.Plugin.Tax.Avalara.Components
{
    /// <summary>
    /// Represents a view component to display applied exemption certificate on the order confirmation page
    /// </summary>
    public class AppliedCertificateViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly IUserService _userService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AppliedCertificateViewComponent(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            IUserService userService,
            IStoreContext storeContext,
            ITaxPluginManager taxPluginManager,
            IWorkContext workContext)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _userService = userService;
            _storeContext = storeContext;
            _taxPluginManager = taxPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            //ensure that Avalara tax provider is active
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, user))
                return Content(string.Empty);

            if (!_avalaraTaxSettings.EnableCertificates)
                return Content(string.Empty);

            //ACL
            if (_avalaraTaxSettings.UserRoleIds.Any())
            {
                var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
                if (!userRoleIds.Intersect(_avalaraTaxSettings.UserRoleIds).Any())
                    return Content(string.Empty);
            }

            //ensure that it's a proper widget zone
            if (!widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
                return Content(string.Empty);

            //ensure that model is passed
            if (additionalData is not ShoppingCartModel cartModel || cartModel.OrderReviewData?.Display != true)
                return Content(string.Empty);

            var store = await _storeContext.GetCurrentStoreAsync();
            var validCertificate = await _avalaraTaxManager.GetValidCertificatesAsync(user, store.Id);
            var certificateValue = !string.IsNullOrEmpty(validCertificate?.exemptionNumber)
                ? validCertificate.exemptionNumber
                : validCertificate?.id?.ToString();

            return View("~/Plugins/Tax.Avalara/Views/Checkout/AppliedCertificate.cshtml", certificateValue);
        }

        #endregion
    }
}