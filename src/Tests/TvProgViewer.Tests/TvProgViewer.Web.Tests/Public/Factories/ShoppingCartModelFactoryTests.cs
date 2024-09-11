using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Data;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Orders;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.ShoppingCart;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class ShoppingCartModelFactoryTests : WebTest
    {
        private IShoppingCartModelFactory _shoppingCartModelFactory;
        private IShoppingCartService _shoppingCartService;
        private IWorkContext _workContext;
        private ITvChannelService _producService;
        private ILocalizationService _localizationService;
        private ShoppingCartItem _shoppingCartItem;
        private ShoppingCartItem _wishlistItem;
        private IUserService _userService;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _shoppingCartModelFactory = GetService<IShoppingCartModelFactory>();
            _shoppingCartService = GetService<IShoppingCartService>();
            _workContext = GetService<IWorkContext>();
            _producService = GetService<ITvChannelService>();
            _localizationService = GetService<ILocalizationService>();
            _userService = GetService<IUserService>();

            var store = await GetService<IStoreContext>().GetCurrentStoreAsync();

            var user = await _workContext.GetCurrentUserAsync();

            _shoppingCartItem = new ShoppingCartItem
            {
                TvChannelId = 1,
                Quantity = 1,
                UserId = user.Id,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                StoreId = store.Id
            };

            _wishlistItem = new ShoppingCartItem
            {
                TvChannelId = 2,
                Quantity = 1,
                UserId = user.Id,
                ShoppingCartType = ShoppingCartType.Wishlist
            };

            var shoppingCartRepo = GetService<IRepository<ShoppingCartItem>>();

            await shoppingCartRepo.InsertAsync(new List<ShoppingCartItem> {_shoppingCartItem, _wishlistItem});

            var currentUser = await _workContext.GetCurrentUserAsync();
            currentUser.HasShoppingCartItems = true;
            await _userService.UpdateUserAsync(currentUser);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _shoppingCartService.DeleteShoppingCartItemAsync(_shoppingCartItem);
            await _shoppingCartService.DeleteShoppingCartItemAsync(_wishlistItem);

            var user = await _workContext.GetCurrentUserAsync();
            user.HasShoppingCartItems = false;
            await _userService.UpdateUserAsync(user);
        }

        [Test]
        public async Task CanPrepareEstimateShippingModel()
        {
            var model = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentUserAsync()));
            
            model.AvailableCountries.Any().Should().BeTrue();
            model.AvailableStates.Any().Should().BeTrue();
            model.Enabled.Should().BeTrue();
            model.ZipPostalCode.Should().Be("10021");
            model.CountryId.Should().BeNull();
            model.StateProvinceId.Should().BeNull();
        }

        [Test]
        public async Task CanPrepareShoppingCartModel()
        {
            var model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(new ShoppingCartModel(),
                new List<ShoppingCartItem> {_shoppingCartItem});

            model.IsEditable.Should().BeTrue();
            model.Items.Any().Should().BeTrue();
            model.Items.Count.Should().Be(1);
            model.Warnings.Count.Should().Be(0);
        }

        [Test]
        public async Task CanPrepareWishlistModel()
        {
            var model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(new WishlistModel(),
                new List<ShoppingCartItem> {_wishlistItem});

            var user = await _workContext.GetCurrentUserAsync();

            model.UserFullname.Should().Be("Smith John Johnovich");
            model.UserGuid.Should().Be(user.UserGuid);
            model.EmailWishlistEnabled.Should().BeTrue();
            model.IsEditable.Should().BeTrue();
            model.Items.Any().Should().BeTrue();
            model.Items.Count.Should().Be(1);
            model.Warnings.Count.Should().Be(0);
        }

        [Test]
        public async Task CanPrepareMiniShoppingCartModel()
        {
            var model = await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync();

            model.CurrentUserIsGuest.Should().BeFalse();
            model.Items.Any().Should().BeTrue();
            model.Items.Count.Should().Be(1);
            model.TotalTvChannels.Should().Be(1);
            model.SubTotal.Should().Be("$1,200.00");
        }

        [Test]
        public async Task CanPrepareOrderTotalsModel()
        {
            var model = await _shoppingCartModelFactory.PrepareOrderTotalsModelAsync(new List<ShoppingCartItem>{_shoppingCartItem}, true);

            model.SubTotal.Should().Be("$1,200.00");
            model.OrderTotal.Should().Be("$1,200.00");

            model.GiftCards.Any().Should().BeFalse();
            model.Shipping.Should().Be("$0.00");
            model.Tax.Should().Be("$0.00");
            model.WillEarnRewardPoints.Should().Be(120);
        }

        [Test]
        public async Task CanPrepareEstimateShippingResultModel()
        {
            var model = await _shoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(new List<ShoppingCartItem> { _shoppingCartItem }, new EstimateShippingModel(), true);
            model.Errors.Any().Should().BeFalse();
        }

        [Test]
        public async Task CanPrepareWishlistEmailAFriendModel()
        {
            var model = await _shoppingCartModelFactory.PrepareWishlistEmailAFriendModelAsync(new WishlistEmailAFriendModel(),
                false);

            model.YourEmailAddress.Should().Be(TvProgTestsDefaults.AdminEmail);
        }

        [Test]
        public async Task CanPrepareCartItemPictureModel()
        {
            var tvChannel = await _producService.GetTvChannelByIdAsync(_shoppingCartItem.TvChannelId);

            var model = await _shoppingCartModelFactory.PrepareCartItemPictureModelAsync(_shoppingCartItem, 100, true, await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name));

            model.AlternateText.Should().Be("Picture of Build your own computer");
            model.ImageUrl.Should()
                .Be($"http://{TvProgTestsDefaults.HostIpAddress}/images/thumbs/0000020_build-your-own-computer_100.jpeg");
            model.Title.Should().Be("Show details for Build your own computer");

            model.FullSizeImageUrl.Should().BeNull();
            model.ThumbImageUrl.Should().BeNull();
        }
    }
}
