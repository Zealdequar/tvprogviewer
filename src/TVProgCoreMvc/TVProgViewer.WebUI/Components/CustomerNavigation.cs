using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class UserNavigationViewComponent : TvProgViewComponent
    {
        private readonly IUserModelFactory _userModelFactory;

        public UserNavigationViewComponent(IUserModelFactory userModelFactory)
        {
            _userModelFactory = userModelFactory;
        }

        public IViewComponentResult Invoke(int selectedTabId = 0)
        {
            var model = _userModelFactory.PrepareUserNavigationModel(selectedTabId);
            return View(model);
        }
    }
}
