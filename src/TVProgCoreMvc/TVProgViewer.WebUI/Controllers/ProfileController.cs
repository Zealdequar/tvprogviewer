using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Web.Framework.Security;

namespace TVProgViewer.WebUI.Controllers
{
    [HttpsRequirement(SslRequirement.No)]
    public partial class ProfileController : BasePublicController
    {
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileController(UserSettings userSettings,
            IUserService userService,
            IPermissionService permissionService,
            IProfileModelFactory profileModelFactory)
        {
            _userSettings = userSettings;
            _userService = userService;
            _permissionService = permissionService;
            _profileModelFactory = profileModelFactory;
        }

        public virtual IActionResult Index(int? id, int? pageNumber)
        {
            if (!_userSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("Homepage");
            }

            var userId = 0;
            if (id.HasValue)
            {
                userId = id.Value;
            }

            var user = _userService.GetUserById(userId);
            if (user == null || _userService.IsGuest(user))
            {
                return RedirectToRoute("Homepage");
            }

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                DisplayEditLink(Url.Action("Edit", "User", new { id = user.Id, area = AreaNames.Admin }));

            var model = _profileModelFactory.PrepareProfileIndexModel(user, pageNumber);
            return View(model);
        }
    }
}