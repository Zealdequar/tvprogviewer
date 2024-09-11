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
    public partial class TvChannelsAlsoPurchasedViewComponent : TvProgViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IOrderReportService _orderReportService;
        private readonly ITvChannelModelFactory _tvChannelModelFactory;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;

        public TvChannelsAlsoPurchasedViewComponent(CatalogSettings catalogSettings,
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

        public async Task<IViewComponentResult> InvokeAsync(int tvChannelId, int? tvChannelThumbPictureSize)
        {
            if (!_catalogSettings.TvChannelsAlsoPurchasedEnabled)
                return Content("");

            //load and cache report
            var store = await _storeContext.GetCurrentStoreAsync();
            var tvChannelIds = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelsAlsoPurchasedIdsKey, tvChannelId, store),
                async () => await _orderReportService.GetAlsoPurchasedTvChannelsIdsAsync(store.Id, tvChannelId, _catalogSettings.TvChannelsAlsoPurchasedNumber)
            );

            //load tvChannels
            var tvChannels = await (await _tvChannelService.GetTvChannelsByIdsAsync(tvChannelIds))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvChannelService.TvChannelIsAvailable(p)).ToListAsync();

            if (!tvChannels.Any())
                return Content("");

            var model = (await _tvChannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvChannels, true, true, tvChannelThumbPictureSize)).ToList();
            return View(model);
        }
    }
}