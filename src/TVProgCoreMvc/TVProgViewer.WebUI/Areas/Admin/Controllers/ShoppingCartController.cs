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

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class ShoppingCartController : BaseAdminController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public ShoppingCartController(IUserService userService,
            IPermissionService permissionService,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IWorkContext workContext)
        {
            _userService = userService;
            _permissionService = permissionService;
            _productService = productService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
        }

        #endregion
        
        #region Methods
        
        public virtual IActionResult CurrentCarts()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            //prepare model
            var model = _shoppingCartModelFactory.PrepareShoppingCartSearchModel(new ShoppingCartSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CurrentCarts(ShoppingCartSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _shoppingCartModelFactory.PrepareShoppingCartListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult ProductSearchAutoComplete(string term)
        {
            const int searchTermMinimumLength = 3;
            if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content(string.Empty);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            //products
            const int productNumber = 15;
            var products = _productService.SearchProducts(
                vendorId: vendorId,
                keywords: term,
                pageSize: productNumber,
                showHidden: true);

            var result = (from p in products
                select new
                {
                    label = p.Name,
                    productid = p.Id
                }).ToList();
            return Json(result);
        }

        [HttpPost]
        public virtual IActionResult GetCartDetails(ShoppingCartItemSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedDataTablesJson();

            //try to get a user with the specified id
            var user = _userService.GetUserById(searchModel.UserId)
                ?? throw new ArgumentException("No user found with the specified id");

            //prepare model
            var model = _shoppingCartModelFactory.PrepareShoppingCartItemListModel(searchModel, user);

            return Json(model);
        }
        
        [HttpPost]
        public virtual IActionResult DeleteItem(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedDataTablesJson();
            
            _shoppingCartService.DeleteShoppingCartItem(id);

            return new NullJsonResult();
        }

        #endregion
    }
}