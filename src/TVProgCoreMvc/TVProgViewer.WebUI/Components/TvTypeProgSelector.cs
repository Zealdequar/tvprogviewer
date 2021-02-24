using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.WebUI.Models.Common;

namespace TVProgViewer.WebUI.Components
{
    public partial class TvTypeProgSelectorViewComponent: TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public TvTypeProgSelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _commonModelFactory.PrepareTvTypeProgSelectorModelAsync();
            return View(model);
        }
    }
}
