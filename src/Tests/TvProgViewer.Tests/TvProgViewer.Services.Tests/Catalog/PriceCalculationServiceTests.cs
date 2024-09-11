using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceCalculationServiceTests : ServiceTest
    {
        #region Fields

        private IUserService _userService;
        private ITvChannelService _tvChannelService;
        private IPriceCalculationService _priceCalcService;

        #endregion

        #region SetUp

        [OneTimeSetUp]
        public void SetUp()
        {
            _userService = GetService<IUserService>();
            _tvChannelService = GetService<ITvChannelService>();
            _priceCalcService = GetService<IPriceCalculationService>();
        }

        #endregion
        
        #region Tests

        [Test]
        public async Task CanGetFinalTvChannelPrice()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("BP_20_WSP");

            var user = new User();
            var store = new Store();

            var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false);
            finalPrice.Should().Be(79.99M);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 2);

            finalPrice.Should().Be(19M);
            finalPriceWithoutDiscounts.Should().Be(finalPriceWithoutDiscounts);
        }

        [Test]
        public async Task CanGetFinalTvChannelPriceWithTierPrices()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("BP_20_WSP");

            var user = new User();
            var store = new Store();

            var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false);
            finalPrice.Should().Be(79.99M);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 2);
            finalPrice.Should().Be(19);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 3);
            finalPrice.Should().Be(19);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 5);
            finalPrice.Should().Be(17);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 7);

            finalPrice.Should().Be(17);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
        }

        [Test]
        public async Task CanGetFinalTvChannelPriceWithTierPricesByUserRole()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("NK_ZSJ_MM");

            //user
            var user = await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail);
            var store = new Store();
                
            var roles = await _userService.GetAllUserRolesAsync();
            var userRole = roles.FirstOrDefault();

            userRole.Should().NotBeNull();

            var tierPrices = new List<TierPrice>
            {
                new TierPrice { UserRoleId = userRole.Id, TvChannelId = tvChannel.Id, Quantity = 2, Price = 25 },
                new TierPrice { UserRoleId = userRole.Id, TvChannelId = tvChannel.Id, Quantity = 5, Price = 20 },
                new TierPrice { UserRoleId = userRole.Id, TvChannelId = tvChannel.Id, Quantity = 10, Price = 15 }
            };

            foreach (var tierPrice in tierPrices) 
                await _tvChannelService.InsertTierPriceAsync(tierPrice);

            tvChannel.HasTierPrices = true;

            var (rezWithoutDiscount1, rez1, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false);
            var (rezWithoutDiscount2, rez2, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 2);
            var (rezWithoutDiscount3, rez3, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 3);
            var (rezWithoutDiscount4, rez4, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 5);
            var (rezWithoutDiscount5, rez5, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 10);
            var (rezWithoutDiscount6, rez6, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 0, false, 15);

            foreach (var tierPrice in tierPrices)
                await _tvChannelService.DeleteTierPriceAsync(tierPrice);

            rez1.Should().Be(30M);
            rez2.Should().Be(25);
            rez3.Should().Be(25);
            rez4.Should().Be(20);
            rez5.Should().Be(15);
            rez6.Should().Be(15);

            rez1.Should().Be(rezWithoutDiscount1);
            rez2.Should().Be(rezWithoutDiscount2);
            rez3.Should().Be(rezWithoutDiscount3);
            rez4.Should().Be(rezWithoutDiscount4);
            rez5.Should().Be(rezWithoutDiscount5);
            rez6.Should().Be(rezWithoutDiscount6);
        }

        [Test]
        public async Task CanGetFinalTvChannelPriceWithAdditionalFee()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("BP_20_WSP");

            //user
            var user = new User();
            var store = new Store();

            var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store, 5, false);

            finalPrice.Should().Be(84.99M);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
        }

        [Test]
        public async Task CanGetFinalTvChannelPriceWithDiscount()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("BP_20_WSP");
            var user = await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail);
            var store = new Store();
            
            var mapping = new DiscountTvChannelMapping
            {
                DiscountId = 1,
                EntityId = tvChannel.Id
            };

            await _tvChannelService.InsertDiscountTvChannelMappingAsync(mapping);
            await _userService.ApplyDiscountCouponCodeAsync(user, "123");

            //set HasDiscountsApplied property
            tvChannel.HasDiscountsApplied = true;
           
            var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(tvChannel, user, store);

            await _tvChannelService.DeleteDiscountTvChannelMappingAsync(mapping);
            await _userService.RemoveDiscountCouponCodeAsync(user, "123");

            finalPrice.Should().Be(69.99M);
            finalPriceWithoutDiscounts.Should().Be(79.99M);
        }

        [TestCase(12.366, 12.37, RoundingType.Rounding001)]
        [TestCase(12.363, 12.36, RoundingType.Rounding001)]
        [TestCase(12.000, 12.00, RoundingType.Rounding001)]
        [TestCase(12.001, 12.00, RoundingType.Rounding001)]
        [TestCase(12.34, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding005Up)]
        [TestCase(12.35, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding005Up)]
        [TestCase(12.05, 12.05, RoundingType.Rounding005Up)]
        [TestCase(12.20, 12.20, RoundingType.Rounding005Up)]
        [TestCase(12.001, 12.00, RoundingType.Rounding005Up)]
        [TestCase(12.34, 12.30, RoundingType.Rounding005Down)]
        [TestCase(12.36, 12.35, RoundingType.Rounding005Down)]
        [TestCase(12.00, 12.00, RoundingType.Rounding005Down)]
        [TestCase(12.05, 12.05, RoundingType.Rounding005Down)]
        [TestCase(12.20, 12.20, RoundingType.Rounding005Down)]
        [TestCase(12.35, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding01Up)]
        [TestCase(12.10, 12.10, RoundingType.Rounding01Up)]
        [TestCase(12.35, 12.30, RoundingType.Rounding01Down)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Down)]
        [TestCase(12.00, 12.00, RoundingType.Rounding01Down)]
        [TestCase(12.10, 12.10, RoundingType.Rounding01Down)]
        [TestCase(12.24, 12.00, RoundingType.Rounding05)]
        [TestCase(12.49, 12.50, RoundingType.Rounding05)]
        [TestCase(12.74, 12.50, RoundingType.Rounding05)]
        [TestCase(12.99, 13.00, RoundingType.Rounding05)]
        [TestCase(12.00, 12.00, RoundingType.Rounding05)]
        [TestCase(12.50, 12.50, RoundingType.Rounding05)]
        [TestCase(12.49, 12.00, RoundingType.Rounding1)]
        [TestCase(12.50, 13.00, RoundingType.Rounding1)]
        [TestCase(12.00, 12.00, RoundingType.Rounding1)]
        [TestCase(12.01, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.99, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding1Up)]
        public void CanRound(decimal valueToRounding, decimal roundedValue, RoundingType roundingType)
        {
            _priceCalcService.Round(valueToRounding, roundingType).Should().Be(roundedValue);
        }

        #endregion
    }
}