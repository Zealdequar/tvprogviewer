using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Data;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Stores;
using TvProgViewer.Tests.TvProgViewer.Services.Tests.Payments;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Orders
{
    [TestFixture]
    public class OrderTotalCalculationServiceTests : ServiceTest
    {
        private IOrderTotalCalculationService _orderTotalCalcService;
        private ITvChannelService _tvChannelService;
        private IUserService _userService;
        private IDiscountService _discountService;
        private TaxSettings _taxSettings;
        private ISettingService _settingService;
        private IShoppingCartService _shoppingCartService;
        private ShoppingCartSettings _shoppingCartSettings;
        private IStoreService _storeService;
        private RewardPointsSettings _rewardPointsSettings;
        private IGenericAttributeService _genericAttributeService;


        private Discount _discount;
        private User _user;
        private Store _store;
        private string _checkoutAttrXml;

        #region Utilities

        private async Task<ShoppingCartItem> CreateTestShopCartItemAsync(decimal tvChannelPrice, int quantity = 1)
        {
            //shopping cart
            var tvChannel = new TvChannel
            {
                Name = "TvChannel name 1",
                Price = tvChannelPrice,
                UserEntersPrice = false,
                Published = true,
                //set HasTierPrices property
                HasTierPrices = true
            };

            await _tvChannelService.InsertTvChannelAsync(tvChannel);

            var shoppingCartItem = new ShoppingCartItem
            {
                UserId = _user.Id,
                TvChannelId = tvChannel.Id,
                StoreId = _store.Id,
                Quantity = quantity
            };

            return shoppingCartItem;
        }

        private async Task<List<ShoppingCartItem>> GetShoppingCartAsync()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            var sci1 = new ShoppingCartItem
            {
                TvChannelId = tvChannel.Id, Quantity = 2
            };
            tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FIRST_PRP");
            var sci2 = new ShoppingCartItem
            {
                TvChannelId = tvChannel.Id, Quantity = 3
            };

            var cart = new List<ShoppingCartItem> {sci1, sci2};
            foreach(var sci in cart)
            {
                sci.UserId = _user.Id;
            }

            return cart;
        }

        #endregion

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _settingService = GetService<ISettingService>();

            var shippingSettings = GetService<ShippingSettings>();
            shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");
            _taxSettings = GetService<TaxSettings>();
            _taxSettings.ActiveTaxProviderSystemName = "FixedTaxRateTest";
            _taxSettings.ShippingIsTaxable = true;
            await _settingService.SaveSettingAsync(shippingSettings);
            await _settingService.SaveSettingAsync(_taxSettings);

            _orderTotalCalcService = GetService<IOrderTotalCalculationService>();
            _tvChannelService = GetService<ITvChannelService>();
            _userService = GetService<IUserService>();
            _discountService = GetService<IDiscountService>();
            _shoppingCartService = GetService<IShoppingCartService>();
            _shoppingCartSettings = GetService<ShoppingCartSettings>();
            _storeService = GetService<IStoreService>();
            _rewardPointsSettings = GetService<RewardPointsSettings>();

            _genericAttributeService= GetService<IGenericAttributeService>();
            var checkoutAttributeService = GetService<ICheckoutAttributeService>();

            var attr = await checkoutAttributeService.GetCheckoutAttributeByIdAsync(1);

            var values = await checkoutAttributeService.GetCheckoutAttributeValuesAsync(attr.Id);

            var val = values.FirstOrDefault(p => p.Name == "Yes")?.Id.ToString();

            _checkoutAttrXml = GetService<ICheckoutAttributeParser>().AddCheckoutAttribute(string.Empty, attr, val);

            _discount = new Discount
            {
                IsActive = true,
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToOrderSubTotal,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            };

            _user = await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail);
            _store = (await _storeService.GetAllStoresAsync()).First();
            
            await GetService<IGenericAttributeService>().SaveAttributeAsync(_user,
                TvProgUserDefaults.SelectedPaymentMethodAttribute, "Payments.TestMethod", 1);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            var settingService = GetService<ISettingService>();

            var shippingSettings = GetService<ShippingSettings>();
            shippingSettings.ActiveShippingRateComputationMethodSystemNames.Clear();

            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            _taxSettings.ActiveTaxProviderSystemName = string.Empty;
            _taxSettings.ShippingIsTaxable = false;
            await settingService.SaveSettingAsync(shippingSettings);
            await settingService.SaveSettingAsync(_taxSettings);

            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.AdditionalShippingCharge = 0M;
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FIRST_PRP");
            tvChannel.AdditionalShippingCharge = 0M;
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            await GetService<IGenericAttributeService>().SaveAttributeAsync<string>(_user, TvProgUserDefaults.SelectedPaymentMethodAttribute, null, 1);
            
            foreach (var item in GetService<IRepository<Discount>>().Table.Where(d => d.Name == "Discount 1").ToList()) 
                await _discountService.DeleteDiscountAsync(item);

            await _tvChannelService.DeleteTvChannelsAsync(GetService<IRepository<TvChannel>>().Table.Where(p => p.Name == "TvChannel name 1").ToList());

            await _genericAttributeService.SaveAttributeAsync<string>(_user, TvProgUserDefaults.CheckoutAttributes, null, _store.Id);
        }

        [Test]
        public async Task CanGetShoppingCartSubTotalExcludingTax()
        {
            //10% - default tax rate
            var(discountAmount, appliedDiscounts, subTotalWithoutDiscount, subTotalWithDiscount, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalAsync(await GetShoppingCartAsync(), false);
            discountAmount.Should().Be(0);
            appliedDiscounts.Count.Should().Be(0);
            subTotalWithoutDiscount.Should().Be(207M);
            subTotalWithDiscount.Should().Be(207M);
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(20.7M);
        }

        [Test]
        public async Task CanGetShoppingCartSubTotalIncludingTax()
        {
            var(discountAmount, appliedDiscounts, subTotalWithoutDiscount, subTotalWithDiscount, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalAsync(await GetShoppingCartAsync(), true);
            discountAmount.Should().Be(0);
            appliedDiscounts.Count.Should().Be(0);
            subTotalWithoutDiscount.Should().Be(227.7M);
            subTotalWithDiscount.Should().Be(227.7M);
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(20.7M);
        }

        [Test]
        public async Task CanGetShoppingCartSubTotal()
        {
            var (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalsAsync(await GetShoppingCartAsync());

            discountAmountExclTax.Should().Be(0);
            subTotalWithoutDiscountExclTax.Should().Be(207M);
            subTotalWithDiscountExclTax.Should().Be(207M);

            discountAmountInclTax.Should().Be(0);
            subTotalWithoutDiscountInclTax.Should().Be(227.7M);
            subTotalWithDiscountInclTax.Should().Be(227.7M);

            appliedDiscounts.Count.Should().Be(0);
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(20.7M);
        }

        [Test]
        public async Task CanGetShoppingCartSubTotalWithCheckoutAttribute()
        {
            var (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalsAsync(await GetShoppingCartAsync());

            discountAmountExclTax.Should().Be(0);
            subTotalWithoutDiscountExclTax.Should().Be(207M);
            subTotalWithDiscountExclTax.Should().Be(207M);

            discountAmountInclTax.Should().Be(0);
            subTotalWithoutDiscountInclTax.Should().Be(227.7M);
            subTotalWithDiscountInclTax.Should().Be(227.7M);

            appliedDiscounts.Count.Should().Be(0);
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(20.7M);

            await _genericAttributeService.SaveAttributeAsync(_user, TvProgUserDefaults.CheckoutAttributes, _checkoutAttrXml, _store.Id);

            (_, _, _, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalsAsync(await GetShoppingCartAsync());

            subTotalWithoutDiscountExclTax.Should().Be(217M);
            subTotalWithDiscountExclTax.Should().Be(217M);

            subTotalWithoutDiscountInclTax.Should().Be(238.7M);
            subTotalWithDiscountInclTax.Should().Be(238.7M);

            await _genericAttributeService.SaveAttributeAsync<string>(_user, TvProgUserDefaults.CheckoutAttributes, null, _store.Id);
        }

        [Test]
        public async Task CanGetShoppingCartSubtotalDiscountExcludingTax()
        {
            await _discountService.InsertDiscountAsync(_discount);

            //10% - default tax rate
            var(discountAmount, appliedDiscounts, subTotalWithoutDiscount, subTotalWithDiscount, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalAsync(await GetShoppingCartAsync(), false);

            await _discountService.DeleteDiscountAsync(_discount);

            discountAmount.Should().Be(3);
            appliedDiscounts.Count.Should().Be(1);
            appliedDiscounts.First().Name.Should().Be("Discount 1");
            subTotalWithoutDiscount.Should().Be(207M);
            subTotalWithDiscount.Should().Be(204M);
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(20.4M);
        }

        [Test]
        public async Task CanGetShoppingCartSubtotalDiscountIncludingTax()
        {
            await _discountService.InsertDiscountAsync(_discount);

            var(discountAmount, appliedDiscounts, subTotalWithoutDiscount, subTotalWithDiscount, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalAsync(await GetShoppingCartAsync(), true);

            await _discountService.DeleteDiscountAsync(_discount);

            //The comparison test failed before, because of a very tiny number difference.
            //discountAmount.ShouldEqual(3.3);
            (Math.Round(discountAmount, 10) == 3.3M).Should().BeTrue();
            appliedDiscounts.Count.Should().Be(1);
            appliedDiscounts.First().Name.Should().Be("Discount 1");
            subTotalWithoutDiscount.Should().Be(227.7M);
            subTotalWithDiscount.Should().Be(224.4M);
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(20.4M);
        }

        [Test]
        public async Task CanGetShoppingCartSubtotalDiscountExcludingAndIncludingTax()
        {
            await _discountService.InsertDiscountAsync(_discount);

            //10% - default tax rate
            var (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalsAsync(await GetShoppingCartAsync());

            await _discountService.DeleteDiscountAsync(_discount);

            discountAmountExclTax.Should().Be(3);
            subTotalWithoutDiscountExclTax.Should().Be(207M);
            subTotalWithDiscountExclTax.Should().Be(204M);

            (Math.Round(discountAmountInclTax, 10) == 3.3M).Should().BeTrue();
            subTotalWithoutDiscountInclTax.Should().Be(227.7M);
            subTotalWithDiscountInclTax.Should().Be(224.4M);

            appliedDiscounts.Count.Should().Be(1);
            appliedDiscounts.First().Name.Should().Be("Discount 1");
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(20.4M);
        }

        [Test]
        public async Task ShippingShouldBeFreeWhenAllShoppingCartItemsAreMarkedAsFreeShipping()
        {
            await TearDown();
            await SetUp();

            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            await _tvChannelService.GetTvChannelBySkuAsync("FIRST_PRP");
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            var isFreeShipping = await _orderTotalCalcService.IsFreeShippingAsync(await GetShoppingCartAsync());
            isFreeShipping.Should().BeTrue();
        }

        [Test]
        public async Task ShippingShouldNotBeFreeWhenSomeOfShoppingCartItemsAreNotMarkedAsFreeShipping()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);
            var isFreeShipping = await _orderTotalCalcService.IsFreeShippingAsync(await GetShoppingCartAsync());
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);
            isFreeShipping.Should().BeFalse();
        }

        [Test]
        public async Task ShippingShouldBeFreeWhenUserIsInRoleWithFreeShipping()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);
            var role = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.AdministratorsRoleName);
            role.FreeShipping = true;
            await _userService.UpdateUserRoleAsync(role);
            var isFreeShipping = await _orderTotalCalcService.IsFreeShippingAsync(await GetShoppingCartAsync());
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);
            role.FreeShipping = false;
            await _userService.UpdateUserRoleAsync(role);
            isFreeShipping.Should().BeTrue();
        }

        [Test]
        public async Task CanGetShippingTotalWithFixedShippingRateExcludingTax()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.AdditionalShippingCharge = 21.25M;
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            var (shipping, taxRate, appliedDiscounts) =
                await _orderTotalCalcService.GetShoppingCartShippingTotalAsync(await GetShoppingCartAsync(), false);

            tvChannel.AdditionalShippingCharge = 0M;
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            shipping.Should().NotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shipping.Should().Be(52.5M);
            appliedDiscounts.Count.Should().Be(0);
            //10 - default fixed tax rate
            taxRate.Should().Be(10);
        }

        [Test]
        public async Task CanGetShippingTotalWithFixedShippingRateIncludingTax()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.AdditionalShippingCharge = 21.25M;
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            var (shipping, taxRate, appliedDiscounts) =
                await _orderTotalCalcService.GetShoppingCartShippingTotalAsync(await GetShoppingCartAsync(), true);

            tvChannel.AdditionalShippingCharge = 0M;
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            shipping.Should().NotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shipping.Should().Be(57.75M);
            appliedDiscounts.Count.Should().Be(0);
            //10 - default fixed tax rate
            taxRate.Should().Be(10);
        }

        [Test]
        public async Task CanGetShippingTotalsWithFixedShippingRate()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.AdditionalShippingCharge = 21.25M;
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            var (shippingInclTax, shippingExclTax, taxRate, appliedDiscounts) =
                await _orderTotalCalcService.GetShoppingCartShippingTotalsAsync(await GetShoppingCartAsync());

            tvChannel.AdditionalShippingCharge = 0M;
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            shippingInclTax.Should().NotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shippingInclTax.Should().Be(57.75M);
            appliedDiscounts.Count.Should().Be(0);
            //10 - default fixed tax rate
            taxRate.Should().Be(10);

            shippingExclTax.Should().NotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shippingExclTax.Should().Be(52.5M);
        }

        [Test]
        public async Task CanGetShippingTotalDiscountExcludingTax()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.AdditionalShippingCharge = 21.25M;
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            _discount.DiscountType = DiscountType.AssignedToShipping;
            await _discountService.InsertDiscountAsync(_discount);

            var (shipping, taxRate, appliedDiscounts) =
                await _orderTotalCalcService.GetShoppingCartShippingTotalAsync(await GetShoppingCartAsync(), false);

            await _discountService.DeleteDiscountAsync(_discount);
            _discount.DiscountType = DiscountType.AssignedToOrderSubTotal;
            tvChannel.AdditionalShippingCharge = 0M;
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            appliedDiscounts.Count.Should().Be(1);
            appliedDiscounts.First().Name.Should().Be("Discount 1");
            shipping.Should().NotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
            shipping.Should().Be(49.5M);
            //10 - default fixed tax rate
            taxRate.Should().Be(10);
        }

        [Test]
        public async Task CanGetShippingTotalDiscountIncludingTax()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.AdditionalShippingCharge = 21.25M;
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            _discount.DiscountType = DiscountType.AssignedToShipping;
            await _discountService.InsertDiscountAsync(_discount);

            var (shipping, taxRate, appliedDiscounts) =
                await _orderTotalCalcService.GetShoppingCartShippingTotalAsync(await GetShoppingCartAsync(), true);

            await _discountService.DeleteDiscountAsync(_discount);
            _discount.DiscountType = DiscountType.AssignedToOrderSubTotal;
            tvChannel.AdditionalShippingCharge = 0M;
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            appliedDiscounts.Count.Should().Be(1);
            appliedDiscounts.First().Name.Should().Be("Discount 1");
            shipping.Should().NotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
            shipping.Should().Be(54.45M);
            //10 - default fixed tax rate
            taxRate.Should().Be(10);
        }

        [Test]
        public async Task CanGetTaxTotal()
        {
            //207 - items, 10 - shipping (fixed), 20 - payment fee

            TestPaymentMethod.AdditionalHandlingFee = 20M;
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            //1. shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            await _settingService.SaveSettingAsync(_taxSettings);

            var(taxTotal, taxRates) = await GetService<IOrderTotalCalculationService>().GetTaxTotalAsync(await GetShoppingCartAsync());
            taxTotal.Should().Be(23.7M);
            taxRates.Should().NotBeNull();
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(23.7M);

            //2. shipping is taxable, payment fee is not taxable
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            await _settingService.SaveSettingAsync(_taxSettings);

            (taxTotal, taxRates) = await GetService<IOrderTotalCalculationService>().GetTaxTotalAsync(await GetShoppingCartAsync());
            taxTotal.Should().Be(21.7M);
            taxRates.Should().NotBeNull();
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(21.7M);

            //3. shipping is not taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = false;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
            await _settingService.SaveSettingAsync(_taxSettings);

            (taxTotal, taxRates) = await GetService<IOrderTotalCalculationService>().GetTaxTotalAsync(await GetShoppingCartAsync());
            taxTotal.Should().Be(22.7M);
            taxRates.Should().NotBeNull();
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(22.7M);

            //4. shipping is not taxable, payment fee is not taxable
            _taxSettings.ShippingIsTaxable = false;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            await _settingService.SaveSettingAsync(_taxSettings);

            (taxTotal, taxRates) = await GetService<IOrderTotalCalculationService>().GetTaxTotalAsync(await GetShoppingCartAsync());
            taxTotal.Should().Be(20.7M);
            taxRates.Should().NotBeNull();
            taxRates.Count.Should().Be(1);
            taxRates.ContainsKey(10).Should().BeTrue();
            taxRates[10].Should().Be(20.7M);

            TestPaymentMethod.AdditionalHandlingFee = 0M;
            tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);
        }

        [Test]
        public async Task CanGetShoppingCartTotalWithoutShippingRequired()
        {
            await TearDown();
            await SetUp();

            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            await _settingService.SaveSettingAsync(_taxSettings);

            TestPaymentMethod.AdditionalHandlingFee = 20M;

            //207 - items, 20 - payment fee, 22.7 - tax
            var (cartTotal, _, _, _, _, _) =
                await _orderTotalCalcService.GetShoppingCartTotalAsync(await GetShoppingCartAsync());
            cartTotal.Should().Be(249.7M);

            TestPaymentMethod.AdditionalHandlingFee = 0M;
        }

        [Test]
        public async Task CanGetShoppingCartTotalWithShippingRequired()
        {
            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            await _settingService.SaveSettingAsync(_taxSettings);

            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            TestPaymentMethod.AdditionalHandlingFee = 20M;

            //207 - items, 10 - shipping (fixed), 20 - payment fee, 23.7 - tax
            var (cartTotal, _, _, _, _, _) = 
                await _orderTotalCalcService.GetShoppingCartTotalAsync(await GetShoppingCartAsync());
            cartTotal.Should().Be(260.7M);

            TestPaymentMethod.AdditionalHandlingFee = 0M;
            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);
        }

        [Test]
        public async Task CanGetShoppingCartItemUnitPrice()
        {
            var items = await GetShoppingCartAsync();
            var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(items[0], true);
            unitPrice.Should().Be(new decimal(27.0));
        }

        [Test]
        public async Task CanGetShoppingCartItemSubtotal()
        {
            var items = await GetShoppingCartAsync();
            var (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(items[0], true);
            subTotal.Should().Be(new decimal(54.0));
        }

        [Test]
        [TestCase(12.00009, 12.00)]
        [TestCase(12.119, 12.12)]
        [TestCase(12.115, 12.12)]
        [TestCase(12.114, 12.11)]
        public async Task TestGetUnitPriceWhenRoundPricesDuringCalculationIsTruePriceMustBeRounded(decimal inputPrice, decimal expectedPrice)
        {
            // arrange
            var shoppingCartItem = await CreateTestShopCartItemAsync(inputPrice);

            // act
            _shoppingCartSettings.RoundPricesDuringCalculation = true;
            await _settingService.SaveSettingAsync(_shoppingCartSettings);

            var (resultPrice, _, _) = await GetService<IShoppingCartService>().GetUnitPriceAsync(shoppingCartItem, true);

            // assert
            resultPrice.Should().Be(expectedPrice);
        }

        [Test]
        [TestCase(12.00009, 12.00009)]
        [TestCase(12.119, 12.119)]
        [TestCase(12.115, 12.115)]
        [TestCase(12.114, 12.114)]
        public async Task TestGetUnitPriceWhenNotRoundPricesDuringCalculationIsFalsePriceMustNotBeRounded(decimal inputPrice, decimal expectedPrice)
        {
            // arrange            
            var shoppingCartItem = await CreateTestShopCartItemAsync(inputPrice);

            // act
            _shoppingCartSettings.RoundPricesDuringCalculation = false;
            await _settingService.SaveSettingAsync(_shoppingCartSettings);

            var (resultPrice, _, _) = await GetService<IShoppingCartService>().GetUnitPriceAsync(shoppingCartItem, true);

            // assert
            resultPrice.Should().Be(expectedPrice);
        }

        [Test]
        public async Task CanGetShoppingCartTotalDiscount()
        {
            _discount.DiscountType = DiscountType.AssignedToOrderTotal;

            await _discountService.InsertDiscountAsync(_discount);

            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            await _settingService.SaveSettingAsync(_taxSettings);

            TestPaymentMethod.AdditionalHandlingFee = 20M;

            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("FR_451_RB");
            tvChannel.IsFreeShipping = false;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            //207 - items, 10 - shipping (fixed), 20 - payment fee, 23.7 - tax, [-3] - discount
            var (scTotal, discountAmount, appliedDiscounts, _, _, _) = await GetService<IOrderTotalCalculationService>().GetShoppingCartTotalAsync(await GetShoppingCartAsync());
            await _discountService.DeleteDiscountAsync(_discount);
            _discount.DiscountType = DiscountType.AssignedToOrderSubTotal;
            TestPaymentMethod.AdditionalHandlingFee = 0M;

            tvChannel.IsFreeShipping = true;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            scTotal.Should().Be(257.7M);
            discountAmount.Should().Be(3);
            appliedDiscounts.Count.Should().Be(1);
            appliedDiscounts.First().Name.Should().Be("Discount 1");
        }

        [Test]
        public async Task CanConvertRewardPointsToAmount()
        {
            _rewardPointsSettings.ExchangeRate = 15M;

            await _settingService.SaveSettingAsync(_rewardPointsSettings);

            var rewardPointsToAmount = await GetService<IOrderTotalCalculationService>().ConvertRewardPointsToAmountAsync(100);

            _rewardPointsSettings.ExchangeRate = 1M;

            await _settingService.SaveSettingAsync(_rewardPointsSettings);

            rewardPointsToAmount.Should().Be(1500);
        }
        
        [Test]
        public async Task CanCheckMinimumRewardPointsToUseRequirement()
        {
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.MinimumRewardPointsToUse = 0;

            await _settingService.SaveSettingAsync(_rewardPointsSettings);

            GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(0).Should().BeTrue();
            GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(1).Should().BeTrue();
            GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(10).Should().BeTrue();

            _rewardPointsSettings.MinimumRewardPointsToUse = 2;
            await _settingService.SaveSettingAsync(_rewardPointsSettings);

            GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(0).Should().BeFalse();
            GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(1).Should().BeFalse();
            GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(2).Should().BeTrue();
            GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(10).Should().BeTrue();
        }
    }
}