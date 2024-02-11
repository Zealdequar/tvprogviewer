using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class CrossSellTvChannelsViewComponent : TvProgViewComponent
    {
        private readonly IAclService _aclService;
        private readonly ITvChannelModelFactory _tvchannelModelFactory;
        private readonly ITvChannelService _tvchannelService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        public CrossSellTvChannelsViewComponent(IAclService aclService,
            ITvChannelModelFactory tvchannelModelFactory,
            ITvChannelService tvchannelService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            _aclService = aclService;
            _tvchannelModelFactory = tvchannelModelFactory;
            _tvchannelService = tvchannelService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
            _shoppingCartSettings = shoppingCartSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? tvchannelThumbPictureSize)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync(), ShoppingCartType.ShoppingCart, store.Id);

            var tvchannels = await (await _tvchannelService.GetCrossSellTvChannelsByShoppingCartAsync(cart, _shoppingCartSettings.CrossSellsNumber))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _tvchannelService.TvChannelIsAvailable(p))
            //visible individually
            .Where(p => p.VisibleIndividually).ToListAsync();

            if (!tvchannels.Any())
                return Content("");

            //Cross-sell tvchannels are displayed on the shopping cart page.
            //We know that the entire shopping cart page is not refresh
            //even if "ShoppingCartSettings.DisplayCartAfterAddingTvChannel" setting  is enabled.
            //That's why we force page refresh (redirect) in this case
            var model = (await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvchannels,
                    tvchannelThumbPictureSize: tvchannelThumbPictureSize, forceRedirectionAfterAddingToCart: true))
                .ToList();

            return View(model);
        }
    }
}