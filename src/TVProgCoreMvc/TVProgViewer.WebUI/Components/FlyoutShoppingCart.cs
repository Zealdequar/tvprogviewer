using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class FlyoutShoppingCartViewComponent : TvProgViewComponent
    {
        private readonly IPermissionService _permissionService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        public FlyoutShoppingCartViewComponent(IPermissionService permissionService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            ShoppingCartSettings shoppingCartSettings)
        {
            _permissionService = permissionService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartSettings = shoppingCartSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_shoppingCartSettings.MiniShoppingCartEnabled)
                return Content("");

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return Content("");

            var model = await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync();
            return View(model);
        }
    }
}
