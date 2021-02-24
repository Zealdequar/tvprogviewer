using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Models.Checkout;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class CheckoutProgressViewComponent : TvProgViewComponent
    {
        private readonly ICheckoutModelFactory _checkoutModelFactory;

        public CheckoutProgressViewComponent(ICheckoutModelFactory checkoutModelFactory)
        {
            _checkoutModelFactory = checkoutModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(CheckoutProgressStep step)
        {
            var model = await _checkoutModelFactory.PrepareCheckoutProgressModelAsync(step);
            return View(model);
        }
    }
}
