using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Components
{
    public class TvProgProviderSelectorViewComponent : TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public TvProgProviderSelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _commonModelFactory.PrepareTvProgProviderSelectorModelAsync();
            return View(model);
        }
    }
}

