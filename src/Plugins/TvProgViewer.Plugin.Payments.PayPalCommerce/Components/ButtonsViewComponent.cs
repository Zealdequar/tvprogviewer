using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Core;
using TvProgViewer.Plugin.Payments.PayPalViewer.Services;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Payments;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.Web.Framework.Infrastructure;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Components
{
    /// <summary>
    /// Represents the view component to display buttons
    /// </summary>
    public class ButtonsViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITvChannelService _tvChannelServise;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly PayPalViewerSettings _settings;

        #endregion

        #region Ctor

        public ButtonsViewComponent(IPaymentPluginManager paymentPluginManager,
            IPriceCalculationService priceCalculationService,
            ITvChannelService tvChannelServise,
            IStoreContext storeContext,
            IWorkContext workContext,
            PayPalViewerSettings settings)
        {
            _paymentPluginManager = paymentPluginManager;
            _priceCalculationService = priceCalculationService;
            _tvChannelServise = tvChannelServise;
            _storeContext = storeContext;
            _workContext = workContext;
            _settings = settings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(PayPalViewerDefaults.SystemName, user, store?.Id ?? 0))
                return Content(string.Empty);

            if (!ServiceManager.IsConfigured(_settings))
                return Content(string.Empty);

            if (!widgetZone.Equals(PublicWidgetZones.TvChannelDetailsAddInfo) && !widgetZone.Equals(PublicWidgetZones.OrderSummaryContentAfter))
                return Content(string.Empty);

            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentAfter))
            {
                if (!_settings.DisplayButtonsOnShoppingCart)
                    return Content(string.Empty);

                var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
                if (routeName != PayPalViewerDefaults.ShoppingCartRouteName)
                    return Content(string.Empty);
            }

            if (widgetZone.Equals(PublicWidgetZones.TvChannelDetailsAddInfo) && !_settings.DisplayButtonsOnTvChannelDetails)
                return Content(string.Empty);

            var tvChannelId = additionalData is TvChannelDetailsModel.AddToCartModel model ? model.TvChannelId : 0;
            var tvChannelCost = "0.00";
            if (tvChannelId > 0)
            {
                var tvChannel = await _tvChannelServise.GetTvChannelByIdAsync(tvChannelId);
                var finalPrice = (await _priceCalculationService.GetFinalPriceAsync(tvChannel, user, store)).finalPrice;
                tvChannelCost = finalPrice.ToString("0.00", CultureInfo.InvariantCulture);
            }
            return View("~/Plugins/Payments.PayPalViewer/Views/Buttons.cshtml", (widgetZone, tvChannelId, tvChannelCost));
        }

        #endregion
    }
}