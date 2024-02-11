using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class RelatedTvChannelsViewComponent : TvProgViewComponent
    {
        private readonly IAclService _aclService;
        private readonly ITvChannelModelFactory _tvchannelModelFactory;
        private readonly ITvChannelService _tvchannelService;
        private readonly IStoreMappingService _storeMappingService;

        public RelatedTvChannelsViewComponent(IAclService aclService,
            ITvChannelModelFactory tvchannelModelFactory,
            ITvChannelService tvchannelService,
            IStoreMappingService storeMappingService)
        {
            _aclService = aclService;
            _tvchannelModelFactory = tvchannelModelFactory;
            _tvchannelService = tvchannelService;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int tvchannelId, int? tvchannelThumbPictureSize)
        {
            //load and cache report
            var tvchannelIds = (await _tvchannelService.GetRelatedTvChannelsByTvChannelId1Async(tvchannelId)).Select(x => x.TvChannelId2).ToArray();

            //load tvchannels
            var tvchannels = await (await _tvchannelService.GetTvChannelsByIdsAsync(tvchannelIds))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvchannelService.TvChannelIsAvailable(p))
            //visible individually
            .Where(p => p.VisibleIndividually).ToListAsync();

            if (!tvchannels.Any())
                return Content(string.Empty);

            var model = (await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvchannels, true, true, tvchannelThumbPictureSize)).ToList();
            return View(model);
        }
    }
}