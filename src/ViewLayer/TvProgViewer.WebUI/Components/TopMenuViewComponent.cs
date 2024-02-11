using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class TopMenuViewComponent : TvProgViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;

        public TopMenuViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            _catalogModelFactory = catalogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? tvchannelThumbPictureSize)
        {
            var model = await _catalogModelFactory.PrepareTopMenuModelAsync();
            return View(model);
        }
    }
}
