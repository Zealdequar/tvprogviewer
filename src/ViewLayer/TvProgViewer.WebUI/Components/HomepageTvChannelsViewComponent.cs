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
    public partial class HomepageTvChannelsViewComponent : TvProgViewComponent
    {
        private readonly IAclService _aclService;
        private readonly ITvChannelModelFactory _tvChannelModelFactory;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStoreMappingService _storeMappingService;

        public HomepageTvChannelsViewComponent(IAclService aclService,
            ITvChannelModelFactory tvChannelModelFactory,
            ITvChannelService tvChannelService,
            IStoreMappingService storeMappingService)
        {
            _aclService = aclService;
            _tvChannelModelFactory = tvChannelModelFactory;
            _tvChannelService = tvChannelService;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? tvChannelThumbPictureSize)
        {
            var tvChannels = await (await _tvChannelService.GetAllTvChannelsDisplayedOnHomepageAsync())
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvChannelService.TvChannelIsAvailable(p))
            //visible individually
            .Where(p => p.VisibleIndividually).ToListAsync();

            if (!tvChannels.Any())
                return Content("");

            var model = (await _tvChannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvChannels, true, true, tvChannelThumbPictureSize)).ToList();

            return View(model);
        }
    }
}