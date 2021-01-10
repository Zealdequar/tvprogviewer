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

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = _userRoleModelFactory.PrepareUserRoleSearchModel(new UserRoleSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(UserRoleSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _userRoleModelFactory.PrepareUserRoleListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers) || !_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = _userRoleModelFactory.PrepareUserRoleModel(new UserRoleModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(UserRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers) || !_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var userRole = model.ToEntity<UserRole>();
                _userService.InsertUserRole(userRole);

                //activity log
                _userActivityService.InsertActivity("AddNewUserRole",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewUserRole"), userRole.Name), userRole);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _userRoleModelFactory.PrepareUserRoleModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers) || !_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a user role with the specified id
            var userRole = _userService.GetUserRoleById(id);
            if (userRole == null)
                return RedirectToAction("List");

            //prepare model
            var model = _userRoleModelFactory.PrepareUserRoleModel(null, userRole);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(UserRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers) || !_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a user role with the specified id
            var userRole = _userService.GetUserRoleById(model.Id);
            if (userRole == null)
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (userRole.IsSystemRole && !model.Active)
                        throw new TvProgException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.Active.CantEditSystem"));

                    if (userRole.IsSystemRole && !userRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new TvProgException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.SystemName.CantEditSystem"));

                    if (TvProgUserDefaults.RegisteredRoleName.Equals(userRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                        model.PurchasedWithProductId > 0)
                        throw new TvProgException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.PurchasedWithProduct.Registered"));

                    userRole = model.ToEntity(userRole);
                    _userService.UpdateUserRole(userRole);

                    //activity log
                    _userActivityService.InsertActivity("EditUserRole",
                        string.Format(_localizationService.GetResource("ActivityLog.EditUserRole"), userRole.Name), userRole);

                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Updated"));

                    return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
                }

                //prepare model
                model = _userRoleModelFactory.PrepareUserRoleModel(model, userRole, true);

                //if we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = userRole.Id });
            }
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers) || !_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a user role with the specified id
            var userRole = _userService.GetUserRoleById(id);
            if (userRole == null)
                return RedirectToAction("List");

            try
            {
                _userService.DeleteUserRole(userRole);

                //activity log
                _userActivityService.InsertActivity("DeleteUserRole",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteUserRole"), userRole.Name), userRole);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = userRole.Id });
            }
        }

        public virtual IActionResult AssociateProductToUserRolePopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers) || !_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = _userRoleModelFactory.PrepareUserRoleProductSearchModel(new UserRoleProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AssociateProductToUserRolePopupList(UserRoleProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers) || !_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _userRoleModelFactory.PrepareUserRoleProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult AssociateProductToUserRolePopup([Bind(Prefix = nameof(AddProductToUserRoleModel))] AddProductToUserRoleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers) || !_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //try to get a product with the specified id
            var associatedProduct = _productService.GetProductById(model.AssociatedToProductId);
            if (associatedProduct == null)
                return Content("Cannot load a product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && associatedProduct.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            ViewBag.RefreshPage = true;
            ViewBag.productId = associatedProduct.Id;
            ViewBag.productName = associatedProduct.Name;

            return View(new UserRoleProductSearchModel());
        }

        #endregion
    }
}