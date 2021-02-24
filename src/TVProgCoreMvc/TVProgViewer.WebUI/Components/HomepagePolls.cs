using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class HomepagePollsViewComponent : TvProgViewComponent
    {
        private readonly IPollModelFactory _pollModelFactory;

        public HomepagePollsViewComponent(IPollModelFactory pollModelFactory)
        {
            _pollModelFactory = pollModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _pollModelFactory.PrepareHomepagePollModelsAsync();
            if (!model.Any())
                return Content("");

            return View(model);
        }
    }
}
