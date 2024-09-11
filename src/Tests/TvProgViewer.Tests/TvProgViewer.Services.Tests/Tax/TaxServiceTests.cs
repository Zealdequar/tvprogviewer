using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Tax;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Tax
{
    [TestFixture]
    public class TaxServiceTests : ServiceTest
    {
        private TaxSettings _taxSettings;
        private bool _defaultEuVatAssumeValid;
        private ITaxPluginManager _taxPluginManager;
        private ISettingService _settingService;
        private ITaxService _taxService;
        private IUserService _userService;
        private bool _defaultAdminRoleTaxExempt;
        private bool _defaultAdminTaxExempt;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _taxService = GetService<ITaxService>();
            _taxPluginManager = GetService<ITaxPluginManager>();
            _taxSettings = GetService<TaxSettings>();
            _settingService = GetService<ISettingService>();
            _userService = GetService<IUserService>();

            _defaultEuVatAssumeValid = _taxSettings.EuVatAssumeValid;
            _taxSettings.EuVatAssumeValid = false;
            await _settingService.SaveSettingAsync(_taxSettings);

            var adminRole = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.AdministratorsRoleName);
            _defaultAdminRoleTaxExempt = adminRole.TaxExempt;
            var admin = await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail);
            _defaultAdminTaxExempt = admin.IsTaxExempt;
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            _taxSettings.EuVatAssumeValid = _defaultEuVatAssumeValid;
            await _settingService.SaveSettingAsync(_taxSettings);

            var adminRole = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.AdministratorsRoleName);
            adminRole.TaxExempt = _defaultAdminRoleTaxExempt;
            adminRole.Active = true;
            await _userService.UpdateUserRoleAsync(adminRole);

            var admin = await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail);
            admin.IsTaxExempt = _defaultAdminTaxExempt;
            await _userService.UpdateUserAsync(admin);
        }

        [Test]
        public async Task CanLoadTaxProviders()
        {
            var providers = await _taxPluginManager.LoadAllPluginsAsync();
            providers.Should().NotBeNull();
            providers.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanLoadTaxProviderBySystemKeyword()
        {
            var provider = await _taxPluginManager.LoadPluginBySystemNameAsync("FixedTaxRateTest");
            provider.Should().NotBeNull();
        }

        [Test]
        public async Task CanLoadActiveTaxProvider()
        {
            var provider = await _taxPluginManager.LoadPrimaryPluginAsync();
            provider.Should().NotBeNull();
        }

        [Test]
        public async Task CanGetTvChannelPricePriceIncludesTaxIncludingTaxTaxable()
        {
            var user = new User();
            var tvChannel = new TvChannel();

            var (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, true, user, true);
            price.Should().Be(1000);
            (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, true, user, false);
            price.Should().Be(1100);
            (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, false, user, true);
            price.Should().Be(909.0909090909090909090909091M);
            (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, false, user, false);
            price.Should().Be(1000);
        }

        [Test]
        public async Task CanGetTvChannelPrice()
        {
            var tvChannel = new TvChannel();
            var user = new User();

            var (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 1000M);
            price.Should().Be(1000);
            (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, true, user, true);
            price.Should().Be(1000);
        }

        [Test]
        public async Task CanGetTvChannelPricePriceIncludesTaxIncludingTaxNonTaxable()
        {
            var user = new User();
            var tvChannel = new TvChannel();

            //not taxable
            user.IsTaxExempt = true;

            var (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, true, user, true);
            price.Should().Be(909.0909090909090909090909091M);
            (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, true, user, false);
            price.Should().Be(1000);
            (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, false, user, true);
            price.Should().Be(909.0909090909090909090909091M);
            (price, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, 0, 1000M, false, user, false);
            price.Should().Be(1000);
        }
    }
}