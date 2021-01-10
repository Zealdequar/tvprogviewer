using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class PollBlockViewComponent : TvProgViewComponent
    {
        private readonly IPollModelFactory _pollModelFactory;

        public PollBlockViewComponent(IPollModelFactory pollModelFactory)
        {
            _pollModelFactory = pollModelFactory;
        }

        public IViewComponentResult Invoke(string systemKeyword)
        {

            if (string.IsNullOrWhiteSpace(systemKeyword))
                return Content("");

            var model = _pollModelFactory.PreparePollModelBySystemName(systemKeyword);
            if (model == null)
                return Content("");

            return View(model);
        }
    }
}
