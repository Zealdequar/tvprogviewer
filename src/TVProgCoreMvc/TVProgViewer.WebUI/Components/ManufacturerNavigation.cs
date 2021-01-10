using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class ManufacturerNavigationViewComponent : TvProgViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly ICatalogModelFactory _catalogModelFactory;

        public ManufacturerNavigationViewComponent(CatalogSettings catalogSettings, ICatalogModelFactory catalogModelFactory)
        {
            _catalogSettings = catalogSettings;
            _catalogModelFactory = catalogModelFactory;
        }

        public IViewComponentResult Invoke(int currentManufacturerId)
        {
            if (_catalogSettings.ManufacturersBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogModelFactory.PrepareManufacturerNavigationModel(currentManufacturerId);
            if (!model.Manufacturers.Any())
                return Content("");

            return View(model);
        }
    }
}
