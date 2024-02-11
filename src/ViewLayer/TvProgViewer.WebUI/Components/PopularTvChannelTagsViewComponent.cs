using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class PopularTvChannelTagsViewComponent : TvProgViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly ICatalogModelFactory _catalogModelFactory;

        public PopularTvChannelTagsViewComponent(CatalogSettings catalogSettings, ICatalogModelFactory catalogModelFactory)
        {
            _catalogSettings = catalogSettings;
            _catalogModelFactory = catalogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _catalogModelFactory.PreparePopularTvChannelTagsModelAsync(_catalogSettings.NumberOfTvChannelTags);

            if (!model.Tags.Any())
                return Content("");

            return View(model);
        }
    }
}
