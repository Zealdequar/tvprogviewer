using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class VendorNavigationViewComponent : TvProgViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly VendorSettings _vendorSettings;

        public VendorNavigationViewComponent(ICatalogModelFactory catalogModelFactory,
            VendorSettings vendorSettings)
        {
            _catalogModelFactory = catalogModelFactory;
            _vendorSettings = vendorSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return Content("");

            var model = await _catalogModelFactory.PrepareVendorNavigationModelAsync();
            if (!model.Vendors.Any())
                return Content("");

            return View(model);
        }
    }
}
