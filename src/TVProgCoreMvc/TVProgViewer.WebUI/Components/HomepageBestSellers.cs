using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Infrastructure.Cache;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class HomepageBestSellersViewComponent : TvProgViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IOrderReportService _orderReportService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;

        public HomepageBestSellersViewComponent(CatalogSettings catalogSettings,
            IAclService aclService,
            IOrderReportService orderReportService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _orderReportService = orderReportService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
        {
            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
                return Content("");

            //load and cache report
            var report = (await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.HomepageBestsellersIdsKey, await _storeContext.GetCurrentStoreAsync()),
                async () => await _orderReportService.BestSellersReportAsync(
                        storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                        pageSize: _catalogSettings.NumberOfBestsellersOnHomepage)))
                    .ToList();

            //load products
            var products = await (await _productService.GetProductsByIdsAsync(report.Select(x => x.ProductId).ToArray()))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p)).ToListAsync();

            if (!products.Any())
                return Content("");

            //prepare model
            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
            return View(model);
        }
    }
}