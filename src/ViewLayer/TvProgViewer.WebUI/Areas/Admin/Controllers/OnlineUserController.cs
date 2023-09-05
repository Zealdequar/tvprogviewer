using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Services.Security;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Models.Users;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
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

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = await _userModelFactory.PrepareOnlineUserSearchModelAsync(new OnlineUserSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(OnlineUserSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _userModelFactory.PrepareOnlineUserListModelAsync(searchModel);

            return Json(model);
        }

        #endregion
    }
}