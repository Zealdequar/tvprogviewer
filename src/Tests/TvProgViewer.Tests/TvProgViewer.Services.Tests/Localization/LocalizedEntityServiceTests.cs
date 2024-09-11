using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Localization;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Localization
{
    [TestFixture]
    public class LocalizedEntityServiceTests : BaseTvProgTest
    {
        private ILocalizedEntityService _localizedEntityService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _localizedEntityService = GetService<ILocalizedEntityService>();
        }

        [Test]
        public async Task CanSaveLocalizedValueAsync()
        {
            var tvChannel = await GetService<ITvChannelService>().GetTvChannelByIdAsync(1);

            await _localizedEntityService.SaveLocalizedValueAsync(tvChannel, p => p.Name, "test lang 1", 1);
            await _localizedEntityService.SaveLocalizedValueAsync(tvChannel, p => p.BasepriceAmount, 1.0M, 1);

            var name = await _localizedEntityService.GetLocalizedValueAsync(1, 1, nameof(TvChannel),
                nameof(TvChannel.Name));

            name.Should().Be("test lang 1");

            var basePriceAmount = await _localizedEntityService.GetLocalizedValueAsync(1, 1, nameof(TvChannel),
                nameof(TvChannel.BasepriceAmount));

            decimal.Parse(basePriceAmount, CultureInfo.InvariantCulture).Should().Be(1M);

            basePriceAmount = await _localizedEntityService.GetLocalizedValueAsync(2, 1, nameof(TvChannel),
                nameof(TvChannel.BasepriceAmount));

            basePriceAmount.Should().BeNullOrEmpty();
        }
    }
}