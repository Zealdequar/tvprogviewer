using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Factories;

namespace TVProgViewer.WebUI.Components
{
    public class TvProgProviderSelectorViewComponent : TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public TvProgProviderSelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareTvProgProviderSelectorModel();
            return View(model);
        }
    }
}

