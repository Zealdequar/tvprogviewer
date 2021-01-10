using System;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Services.Users;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class ProfilePostsViewComponent : TvProgViewComponent
    {
        private readonly IUserService _userService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfilePostsViewComponent(IUserService userService, IProfileModelFactory profileModelFactory)
        {
            _userService = userService;
            _profileModelFactory = profileModelFactory;
        }

        public IViewComponentResult Invoke(int userProfileId, int pageNumber)
        {
            var user = _userService.GetUserById(userProfileId);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var model = _profileModelFactory.PrepareProfilePostsModel(user, pageNumber);
            return View(model);
        }
    }
}
