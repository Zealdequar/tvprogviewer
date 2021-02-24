using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Orders;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    /// <summary>
    /// Represents a estimate shipping view component on shopping cart page
    /// </summary>
    public class ShoppingCartEstimateShippingViewComponent : TvProgViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _shippingSettings;

        public ShoppingCartEstimateShippingViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            ShippingSettings shippingSettings)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _shippingSettings = shippingSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool? prepareAndDisplayOrderReviewData)
        {
            if (!_shippingSettings.EstimateShippingCartPageEnabled)
                return Content("");

            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);

            var model = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(cart);
            if (!model.Enabled)
                return Content("");

            return View(model);
        }
    }
}
