using System;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;
using TVProgViewer.Web.Framework.Controllers;
using TVProgViewer.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class UserRoleController : BaseAdminController
    {
        #region Fields

        private readonly IUserActivityService _userActivityService;
        private readonly IUserRoleModelFactory _userRoleModelFactory;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public UserRoleController(IUserActivityService userActivityService,
            IUserRoleModelFactory userRoleModelFactory,
            IUserService userService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductService productService,
            IWorkContext workContext)
        {
            _userActivityService = userActivityService;
            _userRoleModelFactory = userRoleModelFactory;
            _userService = userService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productService = productService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = await _userRoleModelFactory.PrepareUserRoleSearchModelAsync(new UserRoleSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(UserRoleSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _userRoleModelFactory.PrepareUserRoleListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = await _userRoleModelFactory.PrepareUserRoleModelAsync(new UserRoleModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(UserRoleModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var userRole = model.ToEntity<UserRole>();
                await _userService.InsertUserRoleAsync(userRole);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewUserRole",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewUserRole"), userRole.Name), userRole);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _userRoleModelFactory.PrepareUserRoleModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a user role with the specified id
            var userRole = await _userService.GetUserRoleByIdAsync(id);
            if (userRole == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _userRoleModelFactory.PrepareUserRoleModelAsync(null, userRole);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(UserRoleModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a user role with the specified id
            var userRole = await _userService.GetUserRoleByIdAsync(model.Id);
            if (userRole == null)
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (userRole.IsSystemRole && !model.Active)
                        throw new TvProgException(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Fields.Active.CantEditSystem"));

                    if (userRole.IsSystemRole && !userRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new TvProgException(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Fields.SystemName.CantEditSystem"));

                    if (TvProgUserDefaults.RegisteredRoleName.Equals(userRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                        model.PurchasedWithProductId > 0)
                        throw new TvProgException(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Fields.PurchasedWithProduct.Registered"));

                    userRole = model.ToEntity(userRole);
                    await _userService.UpdateUserRoleAsync(userRole);

                    //activity log
                    await _userActivityService.InsertActivityAsync("EditUserRole",
                        string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditUserRole"), userRole.Name), userRole);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Updated"));

                    return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
                }

                //prepare model
                model = await _userRoleModelFactory.PrepareUserRoleModelAsync(model, userRole, true);

                //if we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = userRole.Id });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a user role with the specified id
            var userRole = await _userService.GetUserRoleByIdAsync(id);
            if (userRole == null)
                return RedirectToAction("List");

            try
            {
                await _userService.DeleteUserRoleAsync(userRole);

                //activity log
                await _userActivityService.InsertActivityAsync("DeleteUserRole",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteUserRole"), userRole.Name), userRole);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.UserRoles.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = userRole.Id });
            }
        }

        public virtual async Task<IActionResult> AssociateProductToUserRolePopup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = await _userRoleModelFactory.PrepareUserRoleProductSearchModelAsync(new UserRoleProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociateProductToUserRolePopupList(UserRoleProductSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _userRoleModelFactory.PrepareUserRoleProductListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociateProductToUserRolePopup([Bind(Prefix = nameof(AddProductToUserRoleModel))] AddProductToUserRoleModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a product with the specified id
            var associatedProduct = await _productService.GetProductByIdAsync(model.AssociatedToProductId);
            if (associatedProduct == null)
                return Content("Cannot load a product");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && associatedProduct.VendorId != (await _workContext.GetCurrentVendorAsync()).Id)
                return Content("This is not your product");

            ViewBag.RefreshPage = true;
            ViewBag.productId = associatedProduct.Id;
            ViewBag.productName = associatedProduct.Name;

            return View(new UserRoleProductSearchModel());
        }

        #endregion
    }
}