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
        private readonly ITvChannelModelFactory _tvChannelModelFactory;
        private readonly ITvChannelService _tvChannelService;
        private readonly IRecentlyViewedTvChannelsService _recentlyViewedTvChannelsService;
        private readonly IStoreMappingService _storeMappingService;

        public RecentlyViewedTvChannelsBlockViewComponent(CatalogSettings catalogSettings,
            IAclService aclService,
            ITvChannelModelFactory tvChannelModelFactory,
            ITvChannelService tvChannelService,
            IRecentlyViewedTvChannelsService recentlyViewedTvChannelsService,
            IStoreMappingService storeMappingService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _tvChannelModelFactory = tvChannelModelFactory;
            _tvChannelService = tvChannelService;
            _recentlyViewedTvChannelsService = recentlyViewedTvChannelsService;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? tvChannelThumbPictureSize, bool? preparePriceModel)
        {
            if (!_catalogSettings.RecentlyViewedTvChannelsEnabled)
                return Content("");

            var preparePictureModel = tvChannelThumbPictureSize.HasValue;
            var tvChannels = await (await _recentlyViewedTvChannelsService.GetRecentlyViewedTvChannelsAsync(_catalogSettings.RecentlyViewedTvChannelsNumber))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvChannelService.TvChannelIsAvailable(p)).ToListAsync();

            if (!tvChannels.Any())
                return Content("");

            //prepare model
            var model = new List<TvChannelOverviewModel>();
            model.AddRange(await _tvChannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvChannels,
                preparePriceModel.GetValueOrDefault(),
                preparePictureModel,
                tvChannelThumbPictureSize));

            return View(model);
        }
    }
}