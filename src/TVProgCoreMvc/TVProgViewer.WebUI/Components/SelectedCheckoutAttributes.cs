using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class SelectedCheckoutAttributesViewComponent : TvProgViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        public SelectedCheckoutAttributesViewComponent(IShoppingCartModelFactory shoppingCartModelFactory)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var attributes = await _shoppingCartModelFactory.FormatSelectedCheckoutAttributesAsync();
            return View(null, attributes);
        }
    }
}
