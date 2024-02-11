using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.WebUI.Components
{
    public partial class RecentlyViewedTvChannelsBlockViewComponent : TvProgViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ITvChannelModelFactory _tvchannelModelFactory;
        private readonly ITvChannelService _tvchannelService;
        private readonly IRecentlyViewedTvChannelsService _recentlyViewedTvChannelsService;
        private readonly IStoreMappingService _storeMappingService;

        public RecentlyViewedTvChannelsBlockViewComponent(CatalogSettings catalogSettings,
            IAclService aclService,
            ITvChannelModelFactory tvchannelModelFactory,
            ITvChannelService tvchannelService,
            IRecentlyViewedTvChannelsService recentlyViewedTvChannelsService,
            IStoreMappingService storeMappingService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _tvchannelModelFactory = tvchannelModelFactory;
            _tvchannelService = tvchannelService;
            _recentlyViewedTvChannelsService = recentlyViewedTvChannelsService;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? tvchannelThumbPictureSize, bool? preparePriceModel)
        {
            if (!_catalogSettings.RecentlyViewedTvChannelsEnabled)
                return Content("");

            var preparePictureModel = tvchannelThumbPictureSize.HasValue;
            var tvchannels = await (await _recentlyViewedTvChannelsService.GetRecentlyViewedTvChannelsAsync(_catalogSettings.RecentlyViewedTvChannelsNumber))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvchannelService.TvChannelIsAvailable(p)).ToListAsync();

            if (!tvchannels.Any())
                return Content("");

            //prepare model
            var model = new List<TvChannelOverviewModel>();
            model.AddRange(await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvchannels,
                preparePriceModel.GetValueOrDefault(),
                preparePictureModel,
                tvchannelThumbPictureSize));

            return View(model);
        }
    }
}