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
        private readonly IProductService _productServise;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly PayPalViewerSettings _settings;

        #endregion

        #region Ctor

        public ButtonsViewComponent(IPaymentPluginManager paymentPluginManager,
            IPriceCalculationService priceCalculationService,
            IProductService productServise,
            IStoreContext storeContext,
            IWorkContext workContext,
            PayPalViewerSettings settings)
        {
            _paymentPluginManager = paymentPluginManager;
            _priceCalculationService = priceCalculationService;
            _productServise = productServise;
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

            if (!widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) && !widgetZone.Equals(PublicWidgetZones.OrderSummaryContentAfter))
                return Content(string.Empty);

            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentAfter))
            {
                if (!_settings.DisplayButtonsOnShoppingCart)
                    return Content(string.Empty);

                var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
                if (routeName != PayPalViewerDefaults.ShoppingCartRouteName)
                    return Content(string.Empty);
            }

            if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) && !_settings.DisplayButtonsOnProductDetails)
                return Content(string.Empty);

            var productId = additionalData is ProductDetailsModel.AddToCartModel model ? model.ProductId : 0;
            var productCost = "0.00";
            if (productId > 0)
            {
                var product = await _productServise.GetProductByIdAsync(productId);
                var finalPrice = (await _priceCalculationService.GetFinalPriceAsync(product, user, store)).finalPrice;
                productCost = finalPrice.ToString("0.00", CultureInfo.InvariantCulture);
            }
            return View("~/Plugins/Payments.PayPalViewer/Views/Buttons.cshtml", (widgetZone, productId, productCost));
        }

        #endregion
    }
}