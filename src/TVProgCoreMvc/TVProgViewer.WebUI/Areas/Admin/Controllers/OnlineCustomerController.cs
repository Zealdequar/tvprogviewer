using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class OnlineUserController : BaseAdminController
    {
        #region Fields

        private readonly IUserModelFactory _userModelFactory;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public OnlineUserController(IUserModelFactory userModelFactory,
            IPermissionService permissionService)
        {
            _userModelFactory = userModelFactory;
            _permissionService = permissionService;
        }

        #endregion
        
        #region Methods

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = _userModelFactory.PrepareOnlineUserSearchModel(new OnlineUserSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(OnlineUserSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _userModelFactory.PrepareOnlineUserListModel(searchModel);

            return Json(model);
        }

        #endregion
    }
}