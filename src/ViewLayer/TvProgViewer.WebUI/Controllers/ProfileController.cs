﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Security;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework;

namespace TvProgViewer.WebUI.Controllers
{
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

        public virtual async Task<IActionResult> Index(int? id, int? pageNumber)
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

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || await _userService.IsGuestAsync(user))
            {
                return RedirectToRoute("Homepage");
            }

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                DisplayEditLink(Url.Action("Edit", "User", new { id = user.Id, area = AreaNames.Admin }));

            var model = await _profileModelFactory.PrepareProfileIndexModelAsync(user, pageNumber);
            return View(model);
        }
    }
}