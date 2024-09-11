using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Shipping;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Shipping
{
    [TestFixture]
    public class ShippingServiceTests : ServiceTest
    {
        #region Fields

        private IShippingPluginManager _shippingPluginManager;
        private IShippingService _shippingService;
        private ITvChannelService _tvChannelService;

        #endregion

        #region Setup

        [OneTimeSetUp]
        public void SetUp()
        {
            _shippingPluginManager = GetService<IShippingPluginManager>();
            _shippingService = GetService<IShippingService>();
            _tvChannelService = GetService<ITvChannelService>();
        } 

        #endregion

        [Test]
        public async Task CanLoadShippingRateComputationMethods()
        {
            var shippingRateComputationMethods = await _shippingPluginManager.LoadAllPluginsAsync();
            shippingRateComputationMethods.Should().NotBeNull();
            shippingRateComputationMethods.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanLoadShippingRateComputationMethodBySystemKeyword()
        {
            var shippingRateComputationMethod = await _shippingPluginManager.LoadPluginBySystemNameAsync("FixedRateTestShippingRateComputationMethod");
            shippingRateComputationMethod.Should().NotBeNull();
        }

        [Test]
        public async Task CanLoadActiveShippingRateComputationMethods()
        {
            var shippingRateComputationMethods = await _shippingPluginManager.LoadActivePluginsAsync(new List<string> { "FixedRateTestShippingRateComputationMethod" });
            shippingRateComputationMethods.Should().NotBeNull();
            shippingRateComputationMethods.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanGetShoppingCartTotalWeightWithoutAttributes()
        {
            var tvChannel1 = await _tvChannelService.GetTvChannelBySkuAsync("AS_551_LP");
            var tvChannel2 = await _tvChannelService.GetTvChannelBySkuAsync("FIRST_PRP");

            var request = new GetShippingOptionRequest
            {
                Items =
                {
                    new GetShippingOptionRequest.PackageItem(
                        new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 3, TvChannelId = tvChannel1.Id },
                        tvChannel1),
                    new GetShippingOptionRequest.PackageItem(
                        new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 4, TvChannelId = tvChannel2.Id },
                        tvChannel2)
                }
            };

            var totalWeight = await _shippingService.GetTotalWeightAsync(request);
            totalWeight.Should().Be(29M);
        }
    }
}
