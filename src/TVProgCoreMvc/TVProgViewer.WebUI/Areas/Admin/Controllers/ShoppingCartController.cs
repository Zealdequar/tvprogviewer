using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Models.ShoppingCart;
using TVProgViewer.Web.Framework.Mvc;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class ShoppingCartController : BaseAdminController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        #endregion

        #region Ctor

        public ShoppingCartController(IUserService userService,
            IPermissionService permissionService,
            IShoppingCartService shoppingCartService,
            IShoppingCartModelFactory shoppingCartModelFactory)
        {
            _userService = userService;
            _permissionService = permissionService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> CurrentCarts()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            //prepare model
            var model = await _shoppingCartModelFactory.PrepareShoppingCartSearchModelAsync(new ShoppingCartSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CurrentCarts(ShoppingCartSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _shoppingCartModelFactory.PrepareShoppingCartListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCartDetails(ShoppingCartItemSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return await AccessDeniedDataTablesJson();

            //try to get a user with the specified id
            var user = await _userService.GetUserByIdAsync(searchModel.UserId)
                ?? throw new ArgumentException("No user found with the specified id");

            //prepare model
            var model = await _shoppingCartModelFactory.PrepareShoppingCartItemListModelAsync(searchModel, user);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteItem(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return await AccessDeniedDataTablesJson();

            await _shoppingCartService.DeleteShoppingCartItemAsync(id);

            return new NullJsonResult();
        }

        #endregion
    }
}