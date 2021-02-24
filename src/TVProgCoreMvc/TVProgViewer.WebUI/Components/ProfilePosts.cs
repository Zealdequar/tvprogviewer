using System;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Services.Users;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

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

        public async Task<IViewComponentResult> InvokeAsync(int userProfileId, int pageNumber)
        {
            var user = await _userService.GetUserByIdAsync(userProfileId);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var model = await _profileModelFactory.PrepareProfilePostsModelAsync(user, pageNumber);
            return View(model);
        }
    }
}
