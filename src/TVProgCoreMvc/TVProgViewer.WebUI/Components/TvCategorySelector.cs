using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Factories;

namespace TVProgViewer.WebUI.Components
{
    public class TvCategorySelectorViewComponent: TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public TvCategorySelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _commonModelFactory.PrepareTvCategorySelectorModelAsync();
            return View(model);
        }
    }
}
