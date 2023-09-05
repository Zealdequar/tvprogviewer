using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.WebUI.Models.Checkout;

namespace TvProgViewer.WebUI.Components
{
    public partial class CheckoutProgressViewComponent : TvProgViewComponent
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
