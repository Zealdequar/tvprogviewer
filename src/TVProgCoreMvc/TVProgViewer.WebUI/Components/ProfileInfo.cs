using System;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Services.Users;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class ProfileInfoViewComponent : TvProgViewComponent
    {
        private readonly IUserService _userService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileInfoViewComponent(IUserService userService, IProfileModelFactory profileModelFactory)
        {
            _userService = userService;
            _profileModelFactory = profileModelFactory;
        }

        public IViewComponentResult Invoke(int userProfileId)
        {
            var user = _userService.GetUserById(userProfileId);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var model = _profileModelFactory.PrepareProfileInfoModel(user);
            return View(model);
        }
    }
}
