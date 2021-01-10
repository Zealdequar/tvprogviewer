using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class SelectedCheckoutAttributesViewComponent : TvProgViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        public SelectedCheckoutAttributesViewComponent(IShoppingCartModelFactory shoppingCartModelFactory)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var attributes = _shoppingCartModelFactory.FormatSelectedCheckoutAttributes();
            return View(null, attributes);
        }
    }
}
