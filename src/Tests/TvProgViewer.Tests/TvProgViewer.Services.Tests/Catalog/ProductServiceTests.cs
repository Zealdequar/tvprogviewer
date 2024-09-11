using System;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Catalog;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Catalog
{
    [TestFixture]
    public class TvChannelServiceTests : ServiceTest
    {
        #region Fields

        private ITvChannelService _tvChannelService;

        #endregion

        #region SetUp

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _tvChannelService = GetService<ITvChannelService>();

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(1);
            tvChannel.ManageInventoryMethod = ManageInventoryMethod.ManageStock;
            tvChannel.UseMultipleWarehouses = true;

            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            await _tvChannelService.InsertTvChannelWarehouseInventoryAsync(new TvChannelWarehouseInventory
            {
                TvChannelId = tvChannel.Id,
                WarehouseId = 1,
                StockQuantity = 8,
                ReservedQuantity = 5
            });

            await _tvChannelService.InsertTvChannelWarehouseInventoryAsync(new TvChannelWarehouseInventory
            {
                TvChannelId = tvChannel.Id,
                WarehouseId = 2,
                StockQuantity = 5
            });
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(1);
            foreach (var tvChannelWarehouseInventory in await _tvChannelService.GetAllTvChannelWarehouseInventoryRecordsAsync(1)) 
                await _tvChannelService.DeleteTvChannelWarehouseInventoryAsync(tvChannelWarehouseInventory);

            tvChannel.ManageInventoryMethod = ManageInventoryMethod.DontManageStock;
            tvChannel.UseMultipleWarehouses = false;

            await _tvChannelService.UpdateTvChannelAsync(tvChannel);
        }

        #endregion

        #region Tests

        [Test]
        public void CanParseRequiredTvChannelIds()
        {
            var tvChannel = new TvChannel
            {
                RequiredTvChannelIds = "1, 4,7 ,a,"
            };

            var ids = _tvChannelService.ParseRequiredTvChannelIds(tvChannel);
            ids.Length.Should().Be(3);
            ids[0].Should().Be(1);
            ids[1].Should().Be(4);
            ids[2].Should().Be(7);
        }

        [Test]
        public void ShouldBeAvailableWhenStartDateIsNotSet()
        {
            var tvChannel = new TvChannel
            {
                AvailableStartDateTimeUtc = null
            };

            _tvChannelService.TvChannelIsAvailable(tvChannel, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldBeAvailableWhenStartDateIsLessThanSomeDate()
        {
            var tvChannel = new TvChannel
            {
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _tvChannelService.TvChannelIsAvailable(tvChannel, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()
        {
            var tvChannel = new TvChannel
            {
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _tvChannelService.TvChannelIsAvailable(tvChannel, new DateTime(2010, 01, 01)).Should().BeFalse();
        }

        [Test]
        public void ShouldBeAvailableWhenEndDateIsNotSet()
        {
            var tvChannel = new TvChannel
            {
                AvailableEndDateTimeUtc = null
            };

            _tvChannelService.TvChannelIsAvailable(tvChannel, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()
        {
            var tvChannel = new TvChannel
            {
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _tvChannelService.TvChannelIsAvailable(tvChannel, new DateTime(2010, 01, 01)).Should().BeTrue();
        }

        [Test]
        public void ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()
        {
            var tvChannel = new TvChannel
            {
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
            };

            _tvChannelService.TvChannelIsAvailable(tvChannel, new DateTime(2010, 01, 03)).Should().BeFalse();
        }

        [Test]
        public void ShouldBeAvailableWhenCurrentDateIsInRange()
        {
            var tvChannel = new TvChannel
            {
                AvailableStartDateTimeUtc = DateTime.UtcNow.AddDays(-1),
                AvailableEndDateTimeUtc = DateTime.UtcNow.AddDays(1)
            };

            _tvChannelService.TvChannelIsAvailable(tvChannel).Should().BeTrue();
        }

        [Test]
        public void ShouldNotBeAvailableWhenCurrentDateIsNotInRange()
        {
            var tvChannel = new TvChannel
            {
                AvailableStartDateTimeUtc = DateTime.UtcNow.AddDays(-2),
                AvailableEndDateTimeUtc = DateTime.UtcNow.AddDays(-1)
            };

            _tvChannelService.TvChannelIsAvailable(tvChannel).Should().BeFalse();
        }

        [Test]
        public void CanParseAllowedQuantities()
        {
            var tvChannel = new TvChannel
            {
                AllowedQuantities = "1, 5,4,10,sdf"
            };

            var result = _tvChannelService.ParseAllowedQuantities(tvChannel);
            result.Length.Should().Be(4);
            result[0].Should().Be(1);
            result[1].Should().Be(5);
            result[2].Should().Be(4);
            result[3].Should().Be(10);
        }

        [Test]
        public async Task CanCalculateTotalQuantityWhenWeDoNotUseMultipleWarehouses()
        {
            var result = await _tvChannelService.GetTotalStockQuantityAsync(new TvChannel { StockQuantity = 6, ManageInventoryMethod = ManageInventoryMethod.ManageStock });
            result.Should().Be(6);
        }

        [Test]
        public async Task PublicVoidCanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithReserved()
        {
            var result = await _tvChannelService.GetTotalStockQuantityAsync(await _tvChannelService.GetTvChannelByIdAsync(1));
            result.Should().Be(8);
        }

        [Test]
        public async Task CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithoutReserved()
        {
            var result = await _tvChannelService.GetTotalStockQuantityAsync(await _tvChannelService.GetTvChannelByIdAsync(1), false);
            result.Should().Be(13);
        }

        [Test]
        public async Task CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithWarehouseSpecified()
        {
            var result = await _tvChannelService.GetTotalStockQuantityAsync(await _tvChannelService.GetTvChannelByIdAsync(1), true, 1);
            result.Should().Be(3);
        }

        [Test]
        public void CanCalculateRentalPeriodsForDays()
        {
            var tvChannel = new TvChannel
            {
                IsRental = true,
                RentalPricePeriod = RentalPricePeriod.Days,
                //rental period length = 1 day
                RentalPriceLength = 1
            };

            //the same date
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //1 day
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).Should().Be(1);
            //2 days
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).Should().Be(2);
            //3 days
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).Should().Be(3);

            //rental period length = 2 days
            tvChannel.RentalPriceLength = 2;
            //the same date
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //1 day
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).Should().Be(1);
            //2 days
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).Should().Be(1);
            //3 days
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).Should().Be(2);
        }

        [Test]
        public void CanCalculateRentalPeriodsForWeeks()
        {
            var tvChannel = new TvChannel
            {
                IsRental = true,
                RentalPricePeriod = RentalPricePeriod.Weeks,
                //rental period length = 1 week
                RentalPriceLength = 1
            };

            //the same date
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a week
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).Should().Be(1);
            //1 week
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).Should().Be(1);
            //several days but less than two weeks
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).Should().Be(2);
            //2 weeks
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).Should().Be(2);
            //3 weeks
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).Should().Be(3);

            //rental period length = 2 weeks
            tvChannel.RentalPriceLength = 2;
            //the same date
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a week
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).Should().Be(1);
            //1 week
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).Should().Be(1);
            //several days but less than two weeks
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).Should().Be(1);
            //2 weeks
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).Should().Be(1);
            //3 weeks
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).Should().Be(2);
        }

        [Test]
        public void CanCalculateRentalPeriodsForMonths()
        {
            var tvChannel = new TvChannel
            {
                IsRental = true,
                RentalPricePeriod = RentalPricePeriod.Months,
                //rental period length = 1 month
                RentalPriceLength = 1
            };

            //the same date
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a month
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).Should().Be(1);
            //1 month
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).Should().Be(1);
            //1 month and 1 day
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 4, 6)).Should().Be(2);
            //several days but less than two months
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).Should().Be(2);
            //2 months
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).Should().Be(2);
            //3 months
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).Should().Be(3);
            //several more unit tests
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)).Should().Be(1);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 1, 1), new DateTime(1900, 1, 2)).Should().Be(1);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 1, 2), new DateTime(1900, 1, 1)).Should().Be(1);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 1, 1), new DateTime(1900, 2, 1)).Should().Be(1);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 2, 1), new DateTime(1900, 1, 1)).Should().Be(1);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 1, 31), new DateTime(1900, 2, 1)).Should().Be(1);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 8, 31), new DateTime(1900, 9, 30)).Should().Be(1);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 8, 31), new DateTime(1900, 10, 1)).Should().Be(2);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 1, 1), new DateTime(1901, 1, 1)).Should().Be(12);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 1, 1), new DateTime(1911, 1, 1)).Should().Be(132);
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(1900, 8, 31), new DateTime(1901, 8, 30)).Should().Be(12);
            
            //rental period length = 2 months
            tvChannel.RentalPriceLength = 2;
            //the same date
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a month
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).Should().Be(1);
            //1 month
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).Should().Be(1);
            //several days but less than two months
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).Should().Be(1);
            //2 months
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).Should().Be(1);
            //3 months
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).Should().Be(2);
        }

        [Test]
        public void CanCalculateRentalPeriodsForYears()
        {
            var tvChannel = new TvChannel
            {
                IsRental = true,
                RentalPricePeriod = RentalPricePeriod.Years,
                //rental period length = 1 years
                RentalPriceLength = 1
            };

            //the same date
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a year
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).Should().Be(1);
            //more than one year
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).Should().Be(2);

            //rental period length = 2 years
            tvChannel.RentalPriceLength = 2;
            //the same date
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
            //several days but less than a year
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).Should().Be(1);
            //more than one year
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).Should().Be(1);
            //more than two year
            _tvChannelService.GetRentalPeriods(tvChannel, new DateTime(2014, 3, 5), new DateTime(2016, 3, 7)).Should().Be(2);
        } 

        #endregion
    }
}
