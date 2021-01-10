using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Orders;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class OrderTotalsViewComponent : TvProgViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public OrderTotalsViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public IViewComponentResult Invoke(bool isEditable)
        {
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentUser, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var model = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, isEditable);
            return View(model);
        }
    }
}
