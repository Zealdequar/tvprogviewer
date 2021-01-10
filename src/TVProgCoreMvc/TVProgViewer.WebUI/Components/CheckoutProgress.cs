using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Models.Checkout;

namespace TVProgViewer.WebUI.Components
{
    public class CheckoutProgressViewComponent : TvProgViewComponent
    {
        private readonly ICheckoutModelFactory _checkoutModelFactory;

        public CheckoutProgressViewComponent(ICheckoutModelFactory checkoutModelFactory)
        {
            _checkoutModelFactory = checkoutModelFactory;
        }

        public IViewComponentResult Invoke(CheckoutProgressStep step)
        {
            var model = _checkoutModelFactory.PrepareCheckoutProgressModel(step);
            return View(model);
        }
    }
}
