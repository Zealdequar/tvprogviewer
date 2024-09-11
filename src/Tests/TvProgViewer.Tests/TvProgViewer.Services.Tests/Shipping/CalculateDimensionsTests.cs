using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Shipping;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Shipping
{
    [TestFixture]
    public class CalculateDimensionsTests : ServiceTest
    {
        private ITvChannelService _tvChannelService;
        private IShippingService _shippingService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _tvChannelService = GetService<ITvChannelService>();
            _shippingService = GetService<IShippingService>();
        }

        [Test]
        public async Task ShouldReturnZeroWithAllZeroDimensions()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("VG_CR_025");
            tvChannel.Length = 0;
            tvChannel.Width = 0;
            tvChannel.Height = 0;

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        Quantity = 1,
                        TvChannelId = tvChannel.Id
                    }, tvChannel)
            };

            var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

            length.Should().Be(0);
            width.Should().Be(0);
            height.Should().Be(0);
            
            items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        Quantity = 2,
                        TvChannelId = tvChannel.Id
                    }, tvChannel)
            };

            (width, length, height) = await _shippingService.GetDimensionsAsync(items);

            length.Should().Be(0);
            width.Should().Be(0);
            height.Should().Be(0);
        }

        [Test]
        public async Task CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("AP_MBP_13");
            tvChannel.Length = 3;
            tvChannel.Width = 2;
            tvChannel.Height = 2;

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 1,
                    TvChannelId = tvChannel.Id
                }, tvChannel)
            };

            var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

            length.Should().Be(3);
            width.Should().Be(2);
            height.Should().Be(2);
        }

        [Test]
        public async Task CanCalculateWithSingleItemAndQty2()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("AP_MBP_13");
            tvChannel.Length = 2;
            tvChannel.Width = 4;
            tvChannel.Height = 4;

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 2,
                    TvChannelId = tvChannel.Id
                }, tvChannel)
            };

            var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

            length.Should().Be(4);
            width.Should().Be(4);
            height.Should().Be(4);
        }

        [Test]
        public async Task CanCalculateWithCubicItemAndMultipleQty()
        {
            var tvChannel = await _tvChannelService.GetTvChannelBySkuAsync("AP_MBP_13");
            tvChannel.Length = 2;
            tvChannel.Width = 2;
            tvChannel.Height = 2;

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 3,
                    TvChannelId = tvChannel.Id
                }, tvChannel)
            };

            var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

            Math.Round(length, 2).Should().Be(2.88M);
            Math.Round(width, 2).Should().Be(2.88M);
            Math.Round(height, 2).Should().Be(2.88M);
        }

        [Test]
        public async Task CanCalculateWithMultipleItems()
        {
            var tvChannel1 = await _tvChannelService.GetTvChannelBySkuAsync("AP_MBP_13");
            tvChannel1.Length = 2;
            tvChannel1.Width = 2;
            tvChannel1.Height = 2;

            var tvChannel2 = await _tvChannelService.GetTvChannelBySkuAsync("VG_CR_025");
            tvChannel2.Length = 3;
            tvChannel2.Width = 5;
            tvChannel2.Height = 2;

            await _tvChannelService.InsertTvChannelAsync(tvChannel2);

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                                {
                                    Quantity = 3,
                                    TvChannelId = tvChannel1.Id
                                }, tvChannel1),
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                                {
                                    Quantity = 1,
                                    TvChannelId = tvChannel2.Id
                                }, tvChannel2)
            };

            var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

            Math.Round(length, 2).Should().Be(3.78M);
            Math.Round(width, 2).Should().Be(5);    //preserve max width
            Math.Round(height, 2).Should().Be(3.78M);

            items.Clear();
            //take 8 cubes of 1x1x1 which is "packed" as 2x2x2 
            for (var i = 0; i < 8; i++)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(i + 1);
                tvChannel.Length = 1;
                tvChannel.Width = 1;
                tvChannel.Height = 1;

                items.Add(new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 1,
                    TvChannelId = tvChannel.Id
                }, tvChannel));
            }

            (width, length, height) = await _shippingService.GetDimensionsAsync(items);

            Math.Round(length, 2).Should().Be(2);
            Math.Round(width, 2).Should().Be(2);
            Math.Round(height, 2).Should().Be(2);
        }
    }
}
