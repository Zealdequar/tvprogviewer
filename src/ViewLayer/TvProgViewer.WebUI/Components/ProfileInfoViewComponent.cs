﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Services.Users;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class ProfileInfoViewComponent : TvProgViewComponent
    {
        private readonly IUserService _userService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileInfoViewComponent(IUserService userService, IProfileModelFactory profileModelFactory)
        {
            _userService = userService;
            _profileModelFactory = profileModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int userProfileId)
        {
            var user = await _userService.GetUserByIdAsync(userProfileId);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var model = await _profileModelFactory.PrepareProfileInfoModelAsync(user);
            return View(model);
        }
    }
}
