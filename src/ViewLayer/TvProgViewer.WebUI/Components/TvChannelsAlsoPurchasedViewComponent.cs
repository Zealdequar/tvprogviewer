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
        private readonly ITvChannelModelFactory _tvchannelModelFactory;
        private readonly ITvChannelService _tvchannelService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;

        public TvChannelsAlsoPurchasedViewComponent(CatalogSettings catalogSettings,
            IAclService aclService,
            IOrderReportService orderReportService,
            ITvChannelModelFactory tvchannelModelFactory,
            ITvChannelService tvchannelService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _orderReportService = orderReportService;
            _tvchannelModelFactory = tvchannelModelFactory;
            _tvchannelService = tvchannelService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int tvchannelId, int? tvchannelThumbPictureSize)
        {
            if (!_catalogSettings.TvChannelsAlsoPurchasedEnabled)
                return Content("");

            //load and cache report
            var store = await _storeContext.GetCurrentStoreAsync();
            var tvchannelIds = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelsAlsoPurchasedIdsKey, tvchannelId, store),
                async () => await _orderReportService.GetAlsoPurchasedTvChannelsIdsAsync(store.Id, tvchannelId, _catalogSettings.TvChannelsAlsoPurchasedNumber)
            );

            //load tvchannels
            var tvchannels = await (await _tvchannelService.GetTvChannelsByIdsAsync(tvchannelIds))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvchannelService.TvChannelIsAvailable(p)).ToListAsync();

            if (!tvchannels.Any())
                return Content("");

            var model = (await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvchannels, true, true, tvchannelThumbPictureSize)).ToList();
            return View(model);
        }
    }
}