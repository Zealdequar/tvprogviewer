using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class UserNavigationViewComponent : TvProgViewComponent
    {
        private readonly IUserModelFactory _userModelFactory;

        public UserNavigationViewComponent(IUserModelFactory userModelFactory)
        {
            _userModelFactory = userModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
        {
            var model = await _userModelFactory.PrepareUserNavigationModelAsync(selectedTabId);
            return View(model);
        }
    }
}
