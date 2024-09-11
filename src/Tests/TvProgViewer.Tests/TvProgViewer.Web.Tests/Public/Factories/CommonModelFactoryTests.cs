using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.Common;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class CommonModelFactoryTests : BaseTvProgTest
    {
        private ICommonModelFactory _commonModelFactory;
        private LocalizationSettings _localizationSettings;
        private IWorkContext _workContext;
        private UserSettings _userSettings;
        private ForumSettings _forumSettings;
        private StoreInformationSettings _storeInformationSettings;
        private NewsSettings _newsSettings;
        private CatalogSettings _catalogSettings;
        private DisplayDefaultFooterItemSettings _displayDefaultFooterItemSettings;
        private CommonSettings _commonSettings;
        private Vendor _vendor;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _commonModelFactory = GetService<ICommonModelFactory>();
            _localizationSettings = GetService<LocalizationSettings>();
            _workContext = GetService<IWorkContext>();
            _userSettings = GetService<UserSettings>();
            _forumSettings = GetService<ForumSettings>();
            _storeInformationSettings = GetService<StoreInformationSettings>();
            _newsSettings = GetService<NewsSettings>();
            _catalogSettings = GetService<CatalogSettings>();
            _commonSettings = GetService<CommonSettings>();
            _displayDefaultFooterItemSettings = GetService<DisplayDefaultFooterItemSettings>();

            _vendor = await GetService<IVendorService>().GetVendorByIdAsync(1);
        }

        [Test]
        public async Task CanPrepareLogoModel()
        {
            var model = await _commonModelFactory.PrepareLogoModelAsync();
            model.StoreName.Should().NotBeNullOrEmpty();
            model.StoreName.Should().Be("TvProgViewer");
            model.LogoPath.Should().NotBeNullOrEmpty();
            model.LogoPath.Should()
                .Be($"http://{TvProgTestsDefaults.HostIpAddress}/Themes/DefaultClean/Content/images/logo.png");
        }

        [Test]
        public async Task CanPrepareLanguageSelectorModel()
        {
            var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

            model.CurrentLanguageId.Should().Be(1);
            model.UseImages.Should().Be(_localizationSettings.UseImagesForLanguageSelection);

            model.AvailableLanguages.Should().NotBeNullOrEmpty();
            var lang = model.AvailableLanguages.FirstOrDefault();
            lang.Should().NotBeNull();
            lang?.Name.Should().Be("EN");
            lang?.FlagImageFileName.Should().Be("us.png");
        }

        [Test]
        public async Task CanPrepareCurrencySelectorModel()
        {
            var model = await _commonModelFactory.PrepareCurrencySelectorModelAsync();
            model.CurrentCurrencyId.Should().Be(1);
            model.AvailableCurrencies.Should().NotBeNullOrEmpty();
            model.AvailableCurrencies.Count.Should().Be(1);
        }

        [Test]
        public async Task CanPrepareTaxTypeSelectorModel()
        {
            var model = await _commonModelFactory.PrepareTaxTypeSelectorModelAsync();
            model.CurrentTaxType.Should().Be(await _workContext.GetTaxDisplayTypeAsync());
        }

        [Test]
        public async Task CanPrepareHeaderLinksModel()
        {
            var model = await _commonModelFactory.PrepareHeaderLinksModelAsync();

            model.RegistrationType.Should().Be(_userSettings.UserRegistrationType);
            model.IsAuthenticated.Should().BeTrue();
            model.UserName.Should().Be("John");
            model.ShoppingCartEnabled.Should().BeTrue();
            model.WishlistEnabled.Should().BeTrue();
            model.AllowPrivateMessages.Should().Be(_forumSettings.AllowPrivateMessages);
            model.UnreadPrivateMessages.Should().BeEmpty();
            model.AlertMessage.Should().BeEmpty();
            model.ShoppingCartItems.Should().Be(0);
            model.WishlistItems.Should().Be(0);
        }

        [Test]
        public async Task CanPrepareAdminHeaderLinksModel()
        {
            var model = await _commonModelFactory.PrepareAdminHeaderLinksModelAsync();
            model.ImpersonatedUserName.Should().Be("John");
            model.IsUserImpersonated.Should().BeFalse();
            model.DisplayAdminLink.Should().BeTrue();
            model.EditPageUrl.Should().BeNull();
        }

        [Test]
        public async Task CanPrepareSocialModel()
        {
            var model = await _commonModelFactory.PrepareSocialModelAsync();

            model.FacebookLink.Should().Be(_storeInformationSettings.FacebookLink);
            model.TwitterLink.Should().Be(_storeInformationSettings.TwitterLink);
            model.YoutubeLink.Should().Be(_storeInformationSettings.YoutubeLink);
            model.InstagramLink.Should().Be(_storeInformationSettings.InstagramLink);
            model.WorkingLanguageId.Should().Be(1);
            model.NewsEnabled.Should().Be(_newsSettings.Enabled);
        }

        [Test]
        public async Task CanPrepareFooterModel()
        {
            var model = await _commonModelFactory.PrepareFooterModelAsync();

            model.StoreName.Should().Be("TvProgViewer");
            model.WishlistEnabled.Should().BeTrue();
            model.ShoppingCartEnabled.Should().BeTrue();
            model.SitemapEnabled.Should().BeTrue();
            model.SearchEnabled.Should().BeTrue();
            model.WorkingLanguageId.Should().Be(1);
            model.BlogEnabled.Should().BeTrue();
            model.CompareTvChannelsEnabled.Should().Be(_catalogSettings.CompareTvChannelsEnabled);
            model.ForumEnabled.Should().Be(_forumSettings.ForumsEnabled);
            model.NewsEnabled.Should().Be(_newsSettings.Enabled);
            model.RecentlyViewedTvChannelsEnabled.Should().Be(_catalogSettings.RecentlyViewedTvChannelsEnabled);
            model.NewTvChannelsEnabled.Should().Be(_catalogSettings.NewTvChannelsEnabled);
            model.DisplayTaxShippingInfoFooter.Should().Be(_catalogSettings.DisplayTaxShippingInfoFooter);
            model.HidePoweredByNopCommerce.Should().Be(_storeInformationSettings.HidePoweredByNopCommerce);
            model.AllowUsersToApplyForVendorAccount.Should().BeTrue();
            model.AllowUsersToCheckGiftCardBalance.Should().BeFalse();
            model.DisplaySitemapFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplaySitemapFooterItem);
            model.DisplayContactUsFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayContactUsFooterItem);
            model.DisplayTvChannelSearchFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayTvChannelSearchFooterItem);
            model.DisplayNewsFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayNewsFooterItem);
            model.DisplayBlogFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayBlogFooterItem);
            model.DisplayForumsFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayForumsFooterItem);
            model.DisplayRecentlyViewedTvChannelsFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayRecentlyViewedTvChannelsFooterItem);
            model.DisplayCompareTvChannelsFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayCompareTvChannelsFooterItem);
            model.DisplayNewTvChannelsFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayNewTvChannelsFooterItem);
            model.DisplayUserInfoFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayUserInfoFooterItem);
            model.DisplayUserOrdersFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayUserOrdersFooterItem);
            model.DisplayUserAddressesFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem);
            model.DisplayShoppingCartFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem);
            model.DisplayWishlistFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayWishlistFooterItem);
            model.DisplayApplyVendorAccountFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem);

            model.Topics.Should().NotBeNullOrEmpty();
            model.Topics.Count.Should().Be(4);
        }

        [Test]
        public async Task CanPrepareContactUsModel()
        {
            var model = new ContactUsModel();
            model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha.Should().BeFalse();
            model.Email.Should().BeNullOrEmpty();
            model.FullName.Should().BeNullOrEmpty();

            model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);
            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha.Should().BeFalse();
            model.Email.Should().Be(TvProgTestsDefaults.AdminEmail);
            model.FullName.Should().Be("Smith John Johnovich");
        }

        [Test]
        public void PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactUsModelAsync(null, true).Wait());

            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactUsModelAsync(null, false).Wait());
        }

        [Test]
        public async Task CanPrepareContactVendorModel()
        {
            var model = new ContactVendorModel();
            model = await _commonModelFactory.PrepareContactVendorModelAsync(model, _vendor, true);
            model.Email.Should().BeNullOrEmpty();
            model.FullName.Should().BeNullOrEmpty();

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha.Should().BeFalse();
            model.VendorId.Should().Be(_vendor.Id);
            model.VendorName.Should().Be(_vendor.Name);

            model = await _commonModelFactory.PrepareContactVendorModelAsync(model, _vendor, false);

            model.Email.Should().Be(TvProgTestsDefaults.AdminEmail);
            model.FullName.Should().Be("Smith John Johnovich");

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha.Should().BeFalse();
            model.VendorId.Should().Be(_vendor.Id);
            model.VendorName.Should().Be(_vendor.Name);
        }

        [Test]
        public void PrepareContactVendorModelShouldRaiseExceptionIfModelOrVendorIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactVendorModelAsync(null, _vendor, true).Wait());

            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactVendorModelAsync(null, _vendor, false).Wait());

            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactVendorModelAsync(new ContactVendorModel(), null, true).Wait());

            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactVendorModelAsync(new ContactVendorModel(), null, false).Wait());
        }

        [Test]
        public async Task CanPrepareStoreThemeSelectorModel()
        {
            var model = await _commonModelFactory.PrepareStoreThemeSelectorModelAsync();
            model.CurrentStoreTheme.Should().NotBeNull();
            model.CurrentStoreTheme.Name.Should().Be("DefaultClean");
            model.CurrentStoreTheme.Title.Should().Be("Default clean");
            model.AvailableStoreThemes.Should().NotBeNull();
            model.AvailableStoreThemes.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CanPrepareFaviconAndAppIconsModel()
        {
            var model = await _commonModelFactory.PrepareFaviconAndAppIconsModelAsync();
            model.HeadCode.Should().Be(_commonSettings.FaviconAndAppIconsHeadCode);
        }

        [Test]
        public async Task CanPrepareRobotsTextFile()
        {
            var model = await _commonModelFactory.PrepareRobotsTextFileAsync();
            model.Should().NotBeNullOrEmpty();
            model.Trim().Split(Environment.NewLine).Length.Should().Be(115);
        }
    }
}
