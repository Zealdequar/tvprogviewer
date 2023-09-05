using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class UserNavigationViewComponent : TvProgViewComponent
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
