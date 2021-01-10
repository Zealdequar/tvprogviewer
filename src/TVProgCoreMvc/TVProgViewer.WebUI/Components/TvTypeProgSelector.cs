using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Factories;

namespace TVProgViewer.WebUI.Components
{
    public partial class TvTypeProgSelectorViewComponent: TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public TvTypeProgSelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareTvTypeProgSelectorModel();
            return View(model);
        }
    }
}
