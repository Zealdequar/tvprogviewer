using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Discounts;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Discounts
{
    [TestFixture]
    public class DiscountServiceTests : ServiceTest
    {
        private IDiscountPluginManager _discountPluginManager;
        private IDiscountService _discountService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _discountPluginManager = GetService<IDiscountPluginManager>();
            _discountService = GetService<IDiscountService>();
        }

        [Test]
        public async Task CanGetAllDiscount()
        {
            var discounts = await _discountService.GetAllDiscountsAsync(isActive: null);
            discounts.Should().NotBeNull();
            discounts.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanLoadDiscountRequirementRules()
        {
            var rules = await _discountPluginManager.LoadAllPluginsAsync();
            rules.Should().NotBeNull();
            rules.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanLoadDiscountRequirementRuleBySystemKeyword()
        {
            var rule = await _discountPluginManager.LoadPluginBySystemNameAsync("TestDiscountRequirementRule");
            rule.Should().NotBeNull();
        }

        [Test]
        public async Task ShouldAcceptValidDiscountCode()
        {
            var discount = CreateDiscount();
            var user = CreateUser();

            var result = await _discountService.ValidateDiscountAsync(discount, user, new[] { "CouponCode 1" });
            result.IsValid.Should().BeTrue();
        }

        private static User CreateUser()
        {
            return new User
            {
                UserGuid = Guid.NewGuid(),
                AdminComment = string.Empty,
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }

        private static Discount CreateDiscount()
        {
            return new Discount
            {
                IsActive = true,
                DiscountType = DiscountType.AssignedToSkus,
                Name = "Discount 2",
                UsePercentage = false,
                DiscountPercentage = 0,
                DiscountAmount = 5,
                RequiresCouponCode = true,
                CouponCode = "CouponCode 1",
                DiscountLimitation = DiscountLimitationType.Unlimited
            };
        }

        [Test]
        public async Task ShouldNotAcceptWrongDiscountCode()
        {
            var discount = CreateDiscount();
            var user = CreateUser();

            var result = await _discountService.ValidateDiscountAsync(discount, user, new[] { "CouponCode 2" });
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public async Task CanValidateDiscountDateRange()
        {
            var discount = CreateDiscount();
            discount.RequiresCouponCode = false;

            var user = CreateUser();

            var result = await _discountService.ValidateDiscountAsync(discount, user, null);
            result.IsValid.Should().BeTrue();

            discount.StartDateUtc = DateTime.UtcNow.AddDays(1);
            result = await _discountService.ValidateDiscountAsync(discount, user, null);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void CanCalculateDiscountAmountPercentage()
        {
            var discount = new Discount
            {
                UsePercentage = true,
                DiscountPercentage = 30
            };

            _discountService.GetDiscountAmount(discount, 100).Should().Be(30);

            discount.DiscountPercentage = 60;
            _discountService.GetDiscountAmount(discount, 200).Should().Be(120);
        }

        [Test]
        public void CanCalculateDiscountAmountFixed()
        {
            var discount = new Discount
            {
                UsePercentage = false,
                DiscountAmount = 10
            };

            _discountService.GetDiscountAmount(discount, 100).Should().Be(10);

            discount.DiscountAmount = 20;
            _discountService.GetDiscountAmount(discount, 200).Should().Be(20);
        }

        [Test]
        public void MaximumDiscountAmountIsUsed()
        {
            var discount = new Discount
            {
                UsePercentage = true,
                DiscountPercentage = 30,
                MaximumDiscountAmount = 3.4M
            };

            _discountService.GetDiscountAmount(discount, 100).Should().Be(3.4M);

            discount.DiscountPercentage = 60;
            _discountService.GetDiscountAmount(discount, 200).Should().Be(3.4M);
            _discountService.GetDiscountAmount(discount, 100).Should().Be(3.4M);

            discount.DiscountPercentage = 1;
            discount.GetDiscountAmount(200).Should().Be(2);
        }
    }

    public static class DiscountExtensions
    {
        private static readonly DiscountService _discountService;

        static DiscountExtensions()
        {
            _discountService = new DiscountService(null, null,
                null, null, null, null, null, null, null, null);
        }

        public static decimal GetDiscountAmount(this Discount discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return _discountService.GetDiscountAmount(discount, amount);

        }
    }
}