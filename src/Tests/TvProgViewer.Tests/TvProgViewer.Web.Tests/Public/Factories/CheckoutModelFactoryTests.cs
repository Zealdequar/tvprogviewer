using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.Tests.TvProgViewer.Services.Tests;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.Checkout;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class CheckoutModelFactoryTests : ServiceTest
    {
        private ICheckoutModelFactory _checkoutModelFactory;
        private IShoppingCartService _shoppingCartService;
        private ITvChannelService _tvChannelService;
        private IList<ShoppingCartItem> _cart;
        private OrderSettings _orderSettings;
        private ShippingSettings _shippingSettings;
        private RewardPointsSettings _rewardPointsSettings;
        private IAddressService _addressService;
        private Address _address;
        private ISettingService _settingService;
        private PaymentSettings _paymentSettings;
        private IRewardPointService _rewardPointService;
        private User _user;
        private IPaymentMethod _paymentMethod;
        private CommonSettings _commonSettings;
        private IOrderService _orderService;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _settingService = GetService<ISettingService>();
            _shippingSettings = GetService<ShippingSettings>();
            _rewardPointsSettings = GetService<RewardPointsSettings>();
            _commonSettings = GetService<CommonSettings>();

            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");
            await _settingService.SaveSettingAsync(_shippingSettings);

            _paymentSettings = GetService<PaymentSettings>();
            _paymentSettings.ActivePaymentMethodSystemNames.Add("Payments.TestMethod");
            await _settingService.SaveSettingAsync(_paymentSettings);

            _user = await GetService<IWorkContext>().GetCurrentUserAsync();

            _rewardPointService = GetService<IRewardPointService>();
            await _rewardPointService.AddRewardPointsHistoryEntryAsync(_user, 10000, 1);

            _shoppingCartService = GetService<IShoppingCartService>();
            _tvChannelService = GetService<ITvChannelService>();
            _addressService = GetService<IAddressService>();

            _address = new Address();

            await _addressService.InsertAddressAsync(_address);
            await GetService<IUserService>().InsertUserAddressAsync(_user, _address);

            _orderSettings = GetService<OrderSettings>();

            await _shoppingCartService.AddToCartAsync(_user, await _tvChannelService.GetTvChannelByIdAsync(1), ShoppingCartType.ShoppingCart, 1);
            await _shoppingCartService.AddToCartAsync(_user, await _tvChannelService.GetTvChannelByIdAsync(2), ShoppingCartType.ShoppingCart, 1);
            await _shoppingCartService.AddToCartAsync(_user, await _tvChannelService.GetTvChannelByIdAsync(3), ShoppingCartType.ShoppingCart, 1);

            _cart = await _shoppingCartService.GetShoppingCartAsync(_user, ShoppingCartType.ShoppingCart);

            _paymentMethod = (await GetService<IPaymentPluginManager>().LoadActivePluginsAsync(new List<string> { "Payments.TestMethod" })).FirstOrDefault();
            _orderService = GetService<IOrderService>();

            _checkoutModelFactory = GetService<ICheckoutModelFactory>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            foreach (var shoppingCartItem in _cart) 
                await _shoppingCartService.DeleteShoppingCartItemAsync(shoppingCartItem);

            await _addressService.DeleteAddressAsync(_address);

            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Clear();
            await _settingService.SaveSettingAsync(_shippingSettings);

            _paymentSettings.ActivePaymentMethodSystemNames.Clear();
            await _settingService.SaveSettingAsync(_paymentSettings);

            _rewardPointsSettings.Enabled = true;
            await _settingService.SaveSettingAsync(_rewardPointsSettings);
            foreach (var history in await _rewardPointService.GetRewardPointsHistoryAsync(_user.Id))
                await _rewardPointService.DeleteRewardPointsHistoryEntryAsync(history);
        }

        [Test]
        public async Task CanPrepareBillingAddressModel()
        {
            var model = await _checkoutModelFactory.PrepareBillingAddressModelAsync(_cart);

            model.ShipToSameAddressAllowed.Should().Be(_shippingSettings.ShipToSameAddress &&
                                                       await _shoppingCartService.ShoppingCartRequiresShippingAsync(_cart));
            model.ShipToSameAddress.Should().Be(!_orderSettings.DisableBillingAddressCheckoutStep);

            model.ExistingAddresses.Any().Should().BeTrue();
            model.ExistingAddresses.Count.Should().Be(1);

            model.InvalidExistingAddresses.Any().Should().BeTrue();
            model.InvalidExistingAddresses.Count.Should().Be(1);

            model.BillingNewAddress.Should().NotBeNull();
        }

        [Test]
        public async Task CanPrepareShippingAddressModel()
        {
            var model = await _checkoutModelFactory.PrepareShippingAddressModelAsync(_cart);

            model.DisplayPickupInStore.Should().Be(!_orderSettings.DisplayPickupInStoreOnShippingMethodPage);

            model.ExistingAddresses.Any().Should().BeTrue();
            model.ExistingAddresses.Count.Should().Be(1);

            model.InvalidExistingAddresses.Any().Should().BeTrue();
            model.InvalidExistingAddresses.Count.Should().Be(1);

            model.ShippingNewAddress.Should().NotBeNull();
        }

        [Test]
        public async Task CanPrepareShippingMethodModel()
        {
            var model = await _checkoutModelFactory.PrepareShippingMethodModelAsync(_cart, await _addressService.GetAddressByIdAsync(1));
            model.DisplayPickupInStore.Should().Be(_orderSettings.DisplayPickupInStoreOnShippingMethodPage);
            model.Warnings.Any().Should().BeFalse();
            model.ShippingMethods.Any().Should().BeTrue();
            model.ShippingMethods.Count.Should().Be(2);
        }

        [Test]
        public async Task CanPreparePaymentMethodModel()
        {
            var model = await _checkoutModelFactory.PreparePaymentMethodModelAsync(_cart, 0);
            
            model.DisplayRewardPoints.Should().BeTrue();
            model.PaymentMethods.Count.Should().Be(1);
            model.RewardPointsToUseAmount.Should().Be("$1,944.90");
            model.RewardPointsToUse.Should().Be(1945);
            model.RewardPointsBalance.Should().Be(10000);
            model.RewardPointsEnoughToPayForOrder.Should().BeTrue();
            model.UseRewardPoints.Should().BeFalse();
        }

        [Test]
        public async Task PreparePaymentMethodModelShouldDependOnSettings()
        {
            var model = await _checkoutModelFactory.PreparePaymentMethodModelAsync(_cart, 0);
            model.DisplayRewardPoints.Should().BeTrue();
            model.RewardPointsToUse.Should().Be(1945);
            model.RewardPointsBalance.Should().Be(10000);

            _rewardPointsSettings.Enabled = false;
            await _settingService.SaveSettingAsync(_rewardPointsSettings);

            model = await GetService<ICheckoutModelFactory>().PreparePaymentMethodModelAsync(_cart, 0);

            _rewardPointsSettings.Enabled = true;
            await _settingService.SaveSettingAsync(_rewardPointsSettings);

            model.DisplayRewardPoints.Should().BeFalse();
            model.RewardPointsBalance.Should().Be(0);
        }

        [Test]
        public async Task CanPreparePaymentInfoModel()
        {
            var model = await _checkoutModelFactory.PreparePaymentInfoModelAsync(_paymentMethod);

            model.PaymentViewComponent.Should().Be(_paymentMethod.GetPublicViewComponent());
            model.DisplayOrderTotals.Should().Be(_orderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab);
        }

        [Test]
        public async Task CanPrepareConfirmOrderModel()
        {
            var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(_cart);

            model.TermsOfServiceOnOrderConfirmPage.Should().Be(_orderSettings.TermsOfServiceOnOrderConfirmPage);
            model.TermsOfServicePopup.Should().Be(_commonSettings.PopupForTermsOfServiceLinks);
            model.MinOrderTotalWarning.Should().BeNullOrEmpty();
            model.Warnings.Any().Should().BeFalse();
        }

        [Test]
        public async Task CanPrepareCheckoutCompletedModel()
        {
            var order = await _orderService.GetOrderByIdAsync(1);
            var model = await _checkoutModelFactory.PrepareCheckoutCompletedModelAsync(order);

            model.OrderId.Should().Be(order.Id);
            model.OnePageCheckoutEnabled.Should().Be(_orderSettings.OnePageCheckoutEnabled);
            model.CustomOrderNumber.Should().Be(order.CustomOrderNumber);
        }

        [Test]
        public void PrepareCheckoutCompletedModelShouldRaiseExceptionIfOrderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _checkoutModelFactory.PrepareCheckoutCompletedModelAsync(null).Wait());
        }

        [Test]
        public async Task CanPrepareCheckoutProgressModel()
        {
            foreach (var step in Enum.GetValues(typeof(CheckoutProgressStep)).Cast<CheckoutProgressStep>())
            {
                var model = await _checkoutModelFactory.PrepareCheckoutProgressModelAsync(step);
                model.CheckoutProgressStep.Should().Be(step);
            }
        }

        [Test]
        public async Task CanPrepareOnePageCheckoutModel()
        {
            var model = await _checkoutModelFactory.PrepareOnePageCheckoutModelAsync(_cart);

            model.ShippingRequired.Should().Be(await _shoppingCartService.ShoppingCartRequiresShippingAsync(_cart));
            model.DisableBillingAddressCheckoutStep.Should().Be(_orderSettings.DisableBillingAddressCheckoutStep);
            model.BillingAddress.Should().NotBeNull();
        }

        [Test]
        public void PrepareOnePageCheckoutModelShouldRaiseExceptionIfCartIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _checkoutModelFactory.PrepareOnePageCheckoutModelAsync(null).Wait());
        }

    }
}
