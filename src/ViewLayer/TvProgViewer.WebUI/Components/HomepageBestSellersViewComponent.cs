using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.WebUI.Infrastructure.Cache;

namespace TvProgViewer.WebUI.Components
{
    public partial class HomepageBestSellersViewComponent : TvProgViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IOrderReportService _orderReportService;
        private readonly ITvChannelModelFactory _tvChannelModelFactory;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;

        public HomepageBestSellersViewComponent(CatalogSettings catalogSettings,
            IAclService aclService,
            IOrderReportService orderReportService,
            ITvChannelModelFactory tvChannelModelFactory,
            ITvChannelService tvChannelService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _orderReportService = orderReportService;
            _tvChannelModelFactory = tvChannelModelFactory;
            _tvChannelService = tvChannelService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? tvChannelThumbPictureSize)
        {
            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
                return Content("");

            //load and cache report
            var store = await _storeContext.GetCurrentStoreAsync();
            var report = await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.HomepageBestsellersIdsKey,
                    store),
                async () => await (await _orderReportService.BestSellersReportAsync(
                    storeId: store.Id,
                    pageSize: _catalogSettings.NumberOfBestsellersOnHomepage)).ToListAsync());

            //load tvChannels
            var tvChannels = await (await _tvChannelService.GetTvChannelsByIdsAsync(report.Select(x => x.TvChannelId).ToArray()))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvChannelService.TvChannelIsAvailable(p)).ToListAsync();

            if (!tvChannels.Any())
                return Content("");

            //prepare model
            var model = (await _tvChannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvChannels, true, true, tvChannelThumbPictureSize)).ToList();
            return View(model);
        }
    }
}