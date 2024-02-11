using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Configuration;
using TvProgViewer.Services;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Gdpr;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Themes;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Settings;
using TvProgViewer.WebUI.Areas.Admin.Models.Stores;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;
using TvProgViewer.Web.Framework.WebOptimizer;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory implementation
    /// </summary>
    public partial class SettingModelFactory : ISettingModelFactory
    {
        #region Fields

        private readonly AppSettings _appSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly IUserAttributeModelFactory _userAttributeModelFactory;
        private readonly ITvProgDataProvider _dataProvider;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGdprService _gdprService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReviewTypeModelFactory _reviewTypeModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IThemeProvider _themeProvider;
        private readonly IVendorAttributeModelFactory _vendorAttributeModelFactory;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SettingModelFactory(AppSettings appSettings,
            CurrencySettings currencySettings,
            IAddressModelFactory addressModelFactory,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICurrencyService currencyService,
            IUserAttributeModelFactory userAttributeModelFactory,
            ITvProgDataProvider dataProvider,
            ITvProgFileProvider fileProvider,
            IDateTimeHelper dateTimeHelper,
            IGdprService gdprService,
            ILocalizedModelFactory localizedModelFactory,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IReturnRequestModelFactory returnRequestModelFactory,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IThemeProvider themeProvider,
            IVendorAttributeModelFactory vendorAttributeModelFactory,
            IReviewTypeModelFactory reviewTypeModelFactory,
            IWorkContext workContext)
        {
            _appSettings = appSettings;
            _currencySettings = currencySettings;
            _addressModelFactory = addressModelFactory;
            _addressAttributeModelFactory = addressAttributeModelFactory;
            _addressService = addressService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _currencyService = currencyService;
            _userAttributeModelFactory = userAttributeModelFactory;
            _dataProvider = dataProvider;
            _fileProvider = fileProvider;
            _dateTimeHelper = dateTimeHelper;
            _gdprService = gdprService;
            _localizedModelFactory = localizedModelFactory;
            _genericAttributeService = genericAttributeService;
            _languageService = languageService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _themeProvider = themeProvider;
            _vendorAttributeModelFactory = vendorAttributeModelFactory;
            _reviewTypeModelFactory = reviewTypeModelFactory;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare store theme models
        /// </summary>
        /// <param name="models">List of store theme models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareStoreThemeModelsAsync(IList<StoreInformationSettingsModel.ThemeModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);

            //get available themes
            var availableThemes = await _themeProvider.GetThemesAsync();
            foreach (var theme in availableThemes)
            {
                models.Add(new StoreInformationSettingsModel.ThemeModel
                {
                    FriendlyName = theme.FriendlyName,
                    SystemName = theme.SystemName,
                    PreviewImageUrl = theme.PreviewImageUrl,
                    PreviewText = theme.PreviewText,
                    SupportRtl = theme.SupportRtl,
                    Selected = theme.SystemName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                });
            }
        }

        /// <summary>
        /// Prepare sort option search model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sort option search model
        /// </returns>
        protected virtual Task<SortOptionSearchModel> PrepareSortOptionSearchModelAsync(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare GDPR consent search model
        /// </summary>
        /// <param name="searchModel">GDPR consent search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent search model
        /// </returns>
        protected virtual Task<GdprConsentSearchModel> PrepareGdprConsentSearchModelAsync(GdprConsentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare address settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address settings model
        /// </returns>
        protected virtual async Task<AddressSettingsModel> PrepareAddressSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>(storeId);

            //fill in model values from the entity
            var model = addressSettings.ToSettingsModel<AddressSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare user settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user settings model
        /// </returns>
        protected virtual async Task<UserSettingsModel> PrepareUserSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var userSettings = await _settingService.LoadSettingAsync<UserSettings>(storeId);

            //fill in model values from the entity
            var model = userSettings.ToSettingsModel<UserSettingsModel>();
            model.AcceptPersonalDataAgreementEnabled = userSettings.AcceptPersonalDataAgreementEnabled;

            return model;
        }

        /// <summary>
        /// Prepare multi-factor authentication settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the multiFactorAuthenticationSettingsModel
        /// </returns>
        protected virtual async Task<MultiFactorAuthenticationSettingsModel> PrepareMultiFactorAuthenticationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>(storeId);

            //fill in model values from the entity
            var model = multiFactorAuthenticationSettings.ToSettingsModel<MultiFactorAuthenticationSettingsModel>();

            return model;

        }

        /// <summary>
        /// Prepare date time settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the date time settings model
        /// </returns>
        protected virtual async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>(storeId);

            //fill in model values from the entity
            var model = new DateTimeSettingsModel
            {
                AllowUsersToSetTimeZone = dateTimeSettings.AllowUsersToSetTimeZone
            };

            //fill in additional values (not existing in the entity)
            model.DefaultStoreGmtZone = _dateTimeHelper.DefaultStoreTimeZone.Id;

            //prepare available time zones
            await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

            return model;
        }

        /// <summary>
        /// Prepare external authentication settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the external authentication settings model
        /// </returns>
        protected virtual async Task<ExternalAuthenticationSettingsModel> PrepareExternalAuthenticationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var externalAuthenticationSettings = await _settingService.LoadSettingAsync<ExternalAuthenticationSettings>(storeId);

            //fill in model values from the entity
            var model = new ExternalAuthenticationSettingsModel
            {
                AllowUsersToRemoveAssociations = externalAuthenticationSettings.AllowUsersToRemoveAssociations
            };

            return model;
        }

        /// <summary>
        /// Prepare store information settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store information settings model
        /// </returns>
        protected virtual async Task<StoreInformationSettingsModel> PrepareStoreInformationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);
            var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

            //fill in model values from the entity
            var model = new StoreInformationSettingsModel
            {
                StoreClosed = storeInformationSettings.StoreClosed,
                DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme,
                AllowUserToSelectTheme = storeInformationSettings.AllowUserToSelectTheme,
                LogoPictureId = storeInformationSettings.LogoPictureId,
                DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning,
                FacebookLink = storeInformationSettings.FacebookLink,
                TwitterLink = storeInformationSettings.TwitterLink,
                YoutubeLink = storeInformationSettings.YoutubeLink,
                InstagramLink = storeInformationSettings.InstagramLink,
                SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm,
                UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm,
                PopupForTermsOfServiceLinks = commonSettings.PopupForTermsOfServiceLinks
            };

            //prepare available themes
            await PrepareStoreThemeModelsAsync(model.AvailableStoreThemes);

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.StoreClosed_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.StoreClosed, storeId);
            model.DefaultStoreTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DefaultStoreTheme, storeId);
            model.AllowUserToSelectTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.AllowUserToSelectTheme, storeId);
            model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.LogoPictureId, storeId);
            model.DisplayEuCookieLawWarning_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeId);
            model.FacebookLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.FacebookLink, storeId);
            model.TwitterLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.TwitterLink, storeId);
            model.YoutubeLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.YoutubeLink, storeId);
            model.InstagramLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.InstagramLink, storeId);
            model.SubjectFieldOnContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.SubjectFieldOnContactUsForm, storeId);
            model.UseSystemEmailForContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.UseSystemEmailForContactUsForm, storeId);
            model.PopupForTermsOfServiceLinks_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.PopupForTermsOfServiceLinks, storeId);

            return model;
        }

        /// <summary>
        /// Prepare Sitemap settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap settings model
        /// </returns>
        protected virtual async Task<SitemapSettingsModel> PrepareSitemapSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>(storeId);

            //fill in model values from the entity
            var model = new SitemapSettingsModel
            {
                SitemapEnabled = sitemapSettings.SitemapEnabled,
                SitemapPageSize = sitemapSettings.SitemapPageSize,
                SitemapIncludeCategories = sitemapSettings.SitemapIncludeCategories,
                SitemapIncludeManufacturers = sitemapSettings.SitemapIncludeManufacturers,
                SitemapIncludeTvChannels = sitemapSettings.SitemapIncludeTvChannels,
                SitemapIncludeTvChannelTags = sitemapSettings.SitemapIncludeTvChannelTags,
                SitemapIncludeBlogPosts = sitemapSettings.SitemapIncludeBlogPosts,
                SitemapIncludeNews = sitemapSettings.SitemapIncludeNews,
                SitemapIncludeTopics = sitemapSettings.SitemapIncludeTopics
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.SitemapEnabled_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapEnabled, storeId);
            model.SitemapPageSize_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapPageSize, storeId);
            model.SitemapIncludeCategories_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeCategories, storeId);
            model.SitemapIncludeManufacturers_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeManufacturers, storeId);
            model.SitemapIncludeTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeTvChannels, storeId);
            model.SitemapIncludeTvChannelTags_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeTvChannelTags, storeId);
            model.SitemapIncludeBlogPosts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeBlogPosts, storeId);
            model.SitemapIncludeNews_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeNews, storeId);
            model.SitemapIncludeTopics_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeTopics, storeId);

            return model;
        }

        /// <summary>
        /// Prepare minification settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the minification settings model
        /// </returns>
        protected virtual async Task<MinificationSettingsModel> PrepareMinificationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var minificationSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

            //fill in model values from the entity
            var model = new MinificationSettingsModel
            {
                EnableHtmlMinification = minificationSettings.EnableHtmlMinification,
                UseResponseCompression = minificationSettings.UseResponseCompression
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.EnableHtmlMinification_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.EnableHtmlMinification, storeId);
            model.UseResponseCompression_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.UseResponseCompression, storeId);

            return model;
        }

        /// <summary>
        /// Prepare SEO settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sEO settings model
        /// </returns>
        protected virtual async Task<SeoSettingsModel> PrepareSeoSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeId);

            //fill in model values from the entity
            var model = new SeoSettingsModel
            {
                PageTitleSeparator = seoSettings.PageTitleSeparator,
                PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
                PageTitleSeoAdjustmentValues = await seoSettings.PageTitleSeoAdjustment.ToSelectListAsync(),
                GenerateTvChannelMetaDescription = seoSettings.GenerateTvChannelMetaDescription,
                ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
                CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
                WwwRequirement = (int)seoSettings.WwwRequirement,
                WwwRequirementValues = await seoSettings.WwwRequirement.ToSelectListAsync(),

                TwitterMetaTags = seoSettings.TwitterMetaTags,
                OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
                CustomHeadTags = seoSettings.CustomHeadTags,
                MicrodataEnabled = seoSettings.MicrodataEnabled
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.PageTitleSeparator_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeparator, storeId);
            model.PageTitleSeoAdjustment_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeoAdjustment, storeId);
            model.GenerateTvChannelMetaDescription_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.GenerateTvChannelMetaDescription, storeId);
            model.ConvertNonWesternChars_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.ConvertNonWesternChars, storeId);
            model.CanonicalUrlsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CanonicalUrlsEnabled, storeId);
            model.WwwRequirement_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.WwwRequirement, storeId);
            model.TwitterMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.TwitterMetaTags, storeId);
            model.OpenGraphMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.OpenGraphMetaTags, storeId);
            model.CustomHeadTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CustomHeadTags, storeId);
            model.MicrodataEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.MicrodataEnabled, storeId);

            return model;
        }

        /// <summary>
        /// Prepare security settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the security settings model
        /// </returns>
        protected virtual async Task<SecuritySettingsModel> PrepareSecuritySettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>(storeId);

            //fill in model values from the entity
            var model = new SecuritySettingsModel
            {
                EncryptionKey = securitySettings.EncryptionKey,
                HoneypotEnabled = securitySettings.HoneypotEnabled
            };

            //fill in additional values (not existing in the entity)
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                model.AdminAreaAllowedIpAddresses = string.Join(",", securitySettings.AdminAreaAllowedIpAddresses);

            return model;
        }

        /// <summary>
        /// Prepare captcha settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the captcha settings model
        /// </returns>
        protected virtual async Task<CaptchaSettingsModel> PrepareCaptchaSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>(storeId);

            //fill in model values from the entity
            var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();

            model.CaptchaTypeValues = await captchaSettings.CaptchaType.ToSelectListAsync();

            if (storeId <= 0)
                return model;

            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.Enabled, storeId);
            model.ShowOnLoginPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnLoginPage, storeId);
            model.ShowOnRegistrationPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnRegistrationPage, storeId);
            model.ShowOnContactUsPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnContactUsPage, storeId);
            model.ShowOnEmailWishlistToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, storeId);
            model.ShowOnEmailTvChannelToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailTvChannelToFriendPage, storeId);
            model.ShowOnBlogCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnBlogCommentPage, storeId);
            model.ShowOnNewsCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnNewsCommentPage, storeId);
            model.ShowOnTvChannelReviewPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnTvChannelReviewPage, storeId);
            model.ShowOnApplyVendorPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnApplyVendorPage, storeId);
            model.ShowOnForgotPasswordPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForgotPasswordPage, storeId);
            model.ShowOnForum_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForum, storeId);
            model.ShowOnCheckoutPageForGuests_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnCheckoutPageForGuests, storeId);
            model.ReCaptchaPublicKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPublicKey, storeId);
            model.ReCaptchaPrivateKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPrivateKey, storeId);
            model.CaptchaType_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.CaptchaType, storeId);
            model.ReCaptchaV3ScoreThreshold_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, storeId);

            return model;
        }

        /// <summary>
        /// Prepare PDF settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pDF settings model
        /// </returns>
        protected virtual async Task<PdfSettingsModel> PreparePdfSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>(storeId);

            //fill in model values from the entity
            var model = new PdfSettingsModel
            {
                LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled,
                LogoPictureId = pdfSettings.LogoPictureId,
                DisablePdfInvoicesForPendingOrders = pdfSettings.DisablePdfInvoicesForPendingOrders,
                InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1,
                InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.LetterPageSizeEnabled_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LetterPageSizeEnabled, storeId);
            model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LogoPictureId, storeId);
            model.DisablePdfInvoicesForPendingOrders_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeId);
            model.InvoiceFooterTextColumn1_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn1, storeId);
            model.InvoiceFooterTextColumn2_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn2, storeId);

            return model;
        }

        /// <summary>
        /// Prepare localization settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localization settings model
        /// </returns>
        protected virtual async Task<LocalizationSettingsModel> PrepareLocalizationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeId);

            //fill in model values from the entity
            var model = new LocalizationSettingsModel
            {
                UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection,
                SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled,
                AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage,
                LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup,
                LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup,
                LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup
            };

            return model;
        }

        /// <summary>
        /// Prepare admin area settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the admin area settings model
        /// </returns>
        protected virtual async Task<AdminAreaSettingsModel> PrepareAdminAreaSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>(storeId);

            //fill in model values from the entity
            var model = new AdminAreaSettingsModel
            {
                UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates
            };

            //fill in overridden values
            if (storeId > 0)
            {
                model.UseRichEditorInMessageTemplates_OverrideForStore = await _settingService.SettingExistsAsync(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, storeId);
            }

            return model;
        }

        /// <summary>
        /// Prepare display default menu item settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the display default menu item settings model
        /// </returns>
        protected virtual async Task<DisplayDefaultMenuItemSettingsModel> PrepareDisplayDefaultMenuItemSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var displayDefaultMenuItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultMenuItemSettings>(storeId);

            //fill in model values from the entity
            var model = new DisplayDefaultMenuItemSettingsModel
            {
                DisplayHomepageMenuItem = displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
                DisplayNewTvChannelsMenuItem = displayDefaultMenuItemSettings.DisplayNewTvChannelsMenuItem,
                DisplayTvChannelSearchMenuItem = displayDefaultMenuItemSettings.DisplayTvChannelSearchMenuItem,
                DisplayUserInfoMenuItem = displayDefaultMenuItemSettings.DisplayUserInfoMenuItem,
                DisplayBlogMenuItem = displayDefaultMenuItemSettings.DisplayBlogMenuItem,
                DisplayForumsMenuItem = displayDefaultMenuItemSettings.DisplayForumsMenuItem,
                DisplayContactUsMenuItem = displayDefaultMenuItemSettings.DisplayContactUsMenuItem
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplayHomepageMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayHomepageMenuItem, storeId);
            model.DisplayNewTvChannelsMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayNewTvChannelsMenuItem, storeId);
            model.DisplayTvChannelSearchMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayTvChannelSearchMenuItem, storeId);
            model.DisplayUserInfoMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayUserInfoMenuItem, storeId);
            model.DisplayBlogMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, storeId);
            model.DisplayForumsMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, storeId);
            model.DisplayContactUsMenuItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, storeId);

            return model;
        }

        /// <summary>
        /// Prepare display default footer item settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the display default footer item settings model
        /// </returns>
        protected virtual async Task<DisplayDefaultFooterItemSettingsModel> PrepareDisplayDefaultFooterItemSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var displayDefaultFooterItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultFooterItemSettings>(storeId);

            //fill in model values from the entity
            var model = new DisplayDefaultFooterItemSettingsModel
            {
                DisplaySitemapFooterItem = displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayTvChannelSearchFooterItem = displayDefaultFooterItemSettings.DisplayTvChannelSearchFooterItem,
                DisplayNewsFooterItem = displayDefaultFooterItemSettings.DisplayNewsFooterItem,
                DisplayBlogFooterItem = displayDefaultFooterItemSettings.DisplayBlogFooterItem,
                DisplayForumsFooterItem = displayDefaultFooterItemSettings.DisplayForumsFooterItem,
                DisplayRecentlyViewedTvChannelsFooterItem = displayDefaultFooterItemSettings.DisplayRecentlyViewedTvChannelsFooterItem,
                DisplayCompareTvChannelsFooterItem = displayDefaultFooterItemSettings.DisplayCompareTvChannelsFooterItem,
                DisplayNewTvChannelsFooterItem = displayDefaultFooterItemSettings.DisplayNewTvChannelsFooterItem,
                DisplayUserInfoFooterItem = displayDefaultFooterItemSettings.DisplayUserInfoFooterItem,
                DisplayUserOrdersFooterItem = displayDefaultFooterItemSettings.DisplayUserOrdersFooterItem,
                DisplayUserAddressesFooterItem = displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem,
                DisplayShoppingCartFooterItem = displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem,
                DisplayWishlistFooterItem = displayDefaultFooterItemSettings.DisplayWishlistFooterItem,
                DisplayApplyVendorAccountFooterItem = displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplaySitemapFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplaySitemapFooterItem, storeId);
            model.DisplayContactUsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayContactUsFooterItem, storeId);
            model.DisplayTvChannelSearchFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayTvChannelSearchFooterItem, storeId);
            model.DisplayNewsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayNewsFooterItem, storeId);
            model.DisplayBlogFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayBlogFooterItem, storeId);
            model.DisplayForumsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayForumsFooterItem, storeId);
            model.DisplayRecentlyViewedTvChannelsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayRecentlyViewedTvChannelsFooterItem, storeId);
            model.DisplayCompareTvChannelsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayCompareTvChannelsFooterItem, storeId);
            model.DisplayNewTvChannelsFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayNewTvChannelsFooterItem, storeId);
            model.DisplayUserInfoFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayUserInfoFooterItem, storeId);
            model.DisplayUserOrdersFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayUserOrdersFooterItem, storeId);
            model.DisplayUserAddressesFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayUserAddressesFooterItem, storeId);
            model.DisplayShoppingCartFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayShoppingCartFooterItem, storeId);
            model.DisplayWishlistFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayWishlistFooterItem, storeId);
            model.DisplayApplyVendorAccountFooterItem_OverrideForStore = await _settingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayApplyVendorAccountFooterItem, storeId);

            return model;
        }

        /// <summary>
        /// Prepare setting model to add
        /// </summary>
        /// <param name="model">Setting model to add</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareAddSettingModelAsync(SettingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
        }

        /// <summary>
        /// Prepare custom HTML settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the custom HTML settings model
        /// </returns>
        protected virtual async Task<CustomHtmlSettingsModel> PrepareCustomHtmlSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

            //fill in model values from the entity
            var model = new CustomHtmlSettingsModel
            {
                HeaderCustomHtml = commonSettings.HeaderCustomHtml,
                FooterCustomHtml = commonSettings.FooterCustomHtml
            };

            //fill in overridden values
            if (storeId > 0)
            {
                model.HeaderCustomHtml_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.HeaderCustomHtml, storeId);
                model.FooterCustomHtml_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.FooterCustomHtml, storeId);
            }

            return model;
        }

        /// <summary>
        /// Prepare robots.txt settings model
        /// </summary>
        /// <param name="model">robots.txt model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the robots.txt settings model
        /// </returns>
        protected virtual async Task<RobotsTxtSettingsModel> PrepareRobotsTxtSettingsModelAsync(RobotsTxtSettingsModel model = null)
        {
            var additionsInstruction =
                string.Format(
                    await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsInstruction"),
                    RobotsTxtDefaults.RobotsAdditionsFileName);

            if (_fileProvider.FileExists(_fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsCustomFileName)))
                return new RobotsTxtSettingsModel { CustomFileExists = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.RobotsCustomFileExists"), RobotsTxtDefaults.RobotsCustomFileName), AdditionsInstruction = additionsInstruction };

            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var robotsTxtSettings = await _settingService.LoadSettingAsync<RobotsTxtSettings>(storeId);
            
            model ??= new RobotsTxtSettingsModel
            {
                AllowSitemapXml = robotsTxtSettings.AllowSitemapXml,
                DisallowPaths = string.Join(Environment.NewLine, robotsTxtSettings.DisallowPaths),
                LocalizableDisallowPaths =
                    string.Join(Environment.NewLine, robotsTxtSettings.LocalizableDisallowPaths),
                DisallowLanguages = robotsTxtSettings.DisallowLanguages.ToList(),
                AdditionsRules = string.Join(Environment.NewLine, robotsTxtSettings.AdditionsRules),
                AvailableLanguages = new List<SelectListItem>()
            };

            if (!model.AvailableLanguages.Any())
                (model.AvailableLanguages as List<SelectListItem>)?.AddRange((await _languageService.GetAllLanguagesAsync(storeId: storeId)).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }));

            model.AdditionsInstruction = additionsInstruction;

            if (storeId <= 0)
                return model;

            model.AdditionsRules_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.AdditionsRules, storeId);
            model.AllowSitemapXml_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.AllowSitemapXml, storeId);
            model.DisallowLanguages_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.DisallowLanguages, storeId);
            model.DisallowPaths_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.DisallowPaths, storeId);
            model.LocalizableDisallowPaths_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.LocalizableDisallowPaths, storeId);

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare app settings model
        /// </summary>
        /// <param name="model">AppSettings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the app settings model
        /// </returns>
        public virtual async Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null)
        {
            model ??= new AppSettingsModel
            {
                CacheConfigModel = _appSettings.Get<CacheConfig>().ToConfigModel<CacheConfigModel>(),
                HostingConfigModel = _appSettings.Get<HostingConfig>().ToConfigModel<HostingConfigModel>(),
                DistributedCacheConfigModel = _appSettings.Get<DistributedCacheConfig>().ToConfigModel<DistributedCacheConfigModel>(),
                AzureBlobConfigModel = _appSettings.Get<AzureBlobConfig>().ToConfigModel<AzureBlobConfigModel>(),
                InstallationConfigModel = _appSettings.Get<InstallationConfig>().ToConfigModel<InstallationConfigModel>(),
                PluginConfigModel = _appSettings.Get<PluginConfig>().ToConfigModel<PluginConfigModel>(),
                CommonConfigModel = _appSettings.Get<CommonConfig>().ToConfigModel<CommonConfigModel>(),
                DataConfigModel = _appSettings.Get<DataConfig>().ToConfigModel<DataConfigModel>(),
                WebOptimizerConfigModel = _appSettings.Get<WebOptimizerConfig>().ToConfigModel<WebOptimizerConfigModel>(),
            };

            model.DistributedCacheConfigModel.DistributedCacheTypeValues = await _appSettings.Get<DistributedCacheConfig>().DistributedCacheType.ToSelectListAsync();

            model.DataConfigModel.DataProviderTypeValues = await _appSettings.Get<DataConfig>().DataProvider.ToSelectListAsync();

            //Since we decided to use the naming of the DB connections section as in the .net core - "ConnectionStrings",
            //we are forced to adjust our internal model naming to this convention in this check.
            model.EnvironmentVariables.AddRange(from property in model.GetType().GetProperties()
                                                where property.Name != nameof(AppSettingsModel.EnvironmentVariables)
                                                from pp in property.PropertyType.GetProperties()
                                                where Environment.GetEnvironmentVariables().Contains($"{property.Name.Replace("Model", "").Replace("DataConfig", "ConnectionStrings")}__{pp.Name}")
                                                select $"{property.Name}_{pp.Name}");
            return model;
        }

        /// <summary>
        /// Prepare blog settings model
        /// </summary>
        /// <param name="model">Blog settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog settings model
        /// </returns>
        public virtual async Task<BlogSettingsModel> PrepareBlogSettingsModelAsync(BlogSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var blogSettings = await _settingService.LoadSettingAsync<BlogSettings>(storeId);

            //fill in model values from the entity
            model ??= blogSettings.ToSettingsModel<BlogSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.Enabled, storeId);
            model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.PostsPageSize, storeId);
            model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
            model.NotifyAboutNewBlogComments_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.NotifyAboutNewBlogComments, storeId);
            model.NumberOfTags_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.NumberOfTags, storeId);
            model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.ShowHeaderRssUrl, storeId);
            model.BlogCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.BlogCommentsMustBeApproved, storeId);

            return model;
        }

        /// <summary>
        /// Prepare vendor settings model
        /// </summary>
        /// <param name="model">Vendor settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor settings model
        /// </returns>
        public virtual async Task<VendorSettingsModel> PrepareVendorSettingsModelAsync(VendorSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var vendorSettings = await _settingService.LoadSettingAsync<VendorSettings>(storeId);

            //fill in model values from the entity
            model ??= vendorSettings.ToSettingsModel<VendorSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.VendorsBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeId);
                model.ShowVendorOnTvChannelDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnTvChannelDetailsPage, storeId);
                model.ShowVendorOnOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnOrderDetailsPage, storeId);
                model.AllowUsersToContactVendors_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowUsersToContactVendors, storeId);
                model.AllowUsersToApplyForVendorAccount_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowUsersToApplyForVendorAccount, storeId);
                model.TermsOfServiceEnabled_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.TermsOfServiceEnabled, storeId);
                model.AllowSearchByVendor_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowSearchByVendor, storeId);
                model.AllowVendorsToEditInfo_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToEditInfo, storeId);
                model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, storeId);
                model.MaximumTvChannelNumber_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.MaximumTvChannelNumber, storeId);
                model.AllowVendorsToImportTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToImportTvChannels, storeId);
            }

            //prepare nested search model
            await _vendorAttributeModelFactory.PrepareVendorAttributeSearchModelAsync(model.VendorAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare forum settings model
        /// </summary>
        /// <param name="model">Forum settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum settings model
        /// </returns>
        public virtual async Task<ForumSettingsModel> PrepareForumSettingsModelAsync(ForumSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>(storeId);

            //fill in model values from the entity
            model ??= forumSettings.ToSettingsModel<ForumSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.ForumEditorValues = await forumSettings.ForumEditor.ToSelectListAsync();

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.ForumsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumsEnabled, storeId);
            model.RelativeDateTimeFormattingEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeId);
            model.ShowUsersPostCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowUsersPostCount, storeId);
            model.AllowGuestsToCreatePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreatePosts, storeId);
            model.AllowGuestsToCreateTopics_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreateTopics, storeId);
            model.AllowUsersToEditPosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowUsersToEditPosts, storeId);
            model.AllowUsersToDeletePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowUsersToDeletePosts, storeId);
            model.AllowPostVoting_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPostVoting, storeId);
            model.MaxVotesPerDay_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.MaxVotesPerDay, storeId);
            model.AllowUsersToManageSubscriptions_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowUsersToManageSubscriptions, storeId);
            model.TopicsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.TopicsPageSize, storeId);
            model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.PostsPageSize, storeId);
            model.ForumEditor_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumEditor, storeId);
            model.SignaturesEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SignaturesEnabled, storeId);
            model.AllowPrivateMessages_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPrivateMessages, storeId);
            model.ShowAlertForPM_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowAlertForPM, storeId);
            model.NotifyAboutPrivateMessages_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.NotifyAboutPrivateMessages, storeId);
            model.ActiveDiscussionsFeedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeId);
            model.ActiveDiscussionsFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedCount, storeId);
            model.ForumFeedsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedsEnabled, storeId);
            model.ForumFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedCount, storeId);
            model.SearchResultsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SearchResultsPageSize, storeId);
            model.ActiveDiscussionsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsPageSize, storeId);

            return model;
        }

        /// <summary>
        /// Prepare news settings model
        /// </summary>
        /// <param name="model">News settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news settings model
        /// </returns>
        public virtual async Task<NewsSettingsModel> PrepareNewsSettingsModelAsync(NewsSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>(storeId);

            //fill in model values from the entity
            model ??= newsSettings.ToSettingsModel<NewsSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.Enabled, storeId);
            model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
            model.NotifyAboutNewNewsComments_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NotifyAboutNewNewsComments, storeId);
            model.ShowNewsOnMainPage_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowNewsOnMainPage, storeId);
            model.MainPageNewsCount_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.MainPageNewsCount, storeId);
            model.NewsArchivePageSize_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NewsArchivePageSize, storeId);
            model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowHeaderRssUrl, storeId);
            model.NewsCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NewsCommentsMustBeApproved, storeId);

            return model;
        }

        /// <summary>
        /// Prepare shipping settings model
        /// </summary>
        /// <param name="model">Shipping settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping settings model
        /// </returns>
        public virtual async Task<ShippingSettingsModel> PrepareShippingSettingsModelAsync(ShippingSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var shippingSettings = await _settingService.LoadSettingAsync<ShippingSettings>(storeId);

            //fill in model values from the entity
            model ??= shippingSettings.ToSettingsModel<ShippingSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.SortShippingValues = await shippingSettings.ShippingSorting.ToSelectListAsync();

            //fill in overridden values
            if (storeId > 0)
            {
                model.ShipToSameAddress_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShipToSameAddress, storeId);
                model.AllowPickupInStore_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.AllowPickupInStore, storeId);
                model.DisplayPickupPointsOnMap_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayPickupPointsOnMap, storeId);
                model.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.IgnoreAdditionalShippingChargeForPickupInStore, storeId);
                model.GoogleMapsApiKey_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.GoogleMapsApiKey, storeId);
                model.UseWarehouseLocation_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.UseWarehouseLocation, storeId);
                model.NotifyUserAboutShippingFromMultipleLocations_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.NotifyUserAboutShippingFromMultipleLocations, storeId);
                model.FreeShippingOverXEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXEnabled, storeId);
                model.FreeShippingOverXValue_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXValue, storeId);
                model.FreeShippingOverXIncludingTax_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeId);
                model.EstimateShippingCartPageEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingCartPageEnabled, storeId);
                model.EstimateShippingTvChannelPageEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingTvChannelPageEnabled, storeId);
                model.EstimateShippingCityNameEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingCityNameEnabled, storeId);
                model.DisplayShipmentEventsToUsers_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayShipmentEventsToUsers, storeId);
                model.DisplayShipmentEventsToStoreOwner_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, storeId);
                model.HideShippingTotal_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.HideShippingTotal, storeId);
                model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, storeId);
                model.ConsiderAssociatedTvChannelsDimensions_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ConsiderAssociatedTvChannelsDimensions, storeId);
                model.ShippingOriginAddress_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShippingOriginAddressId, storeId);
                model.ShippingSorting_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShippingSorting, storeId);
            }

            //prepare shipping origin address
            var originAddress = await _addressService.GetAddressByIdAsync(shippingSettings.ShippingOriginAddressId);
            if (originAddress != null)
                model.ShippingOriginAddress = originAddress.ToModel(model.ShippingOriginAddress);
            await _addressModelFactory.PrepareAddressModelAsync(model.ShippingOriginAddress, originAddress);
            model.ShippingOriginAddress.ZipPostalCodeRequired = true;

            return model;
        }

        /// <summary>
        /// Prepare tax settings model
        /// </summary>
        /// <param name="model">Tax settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax settings model
        /// </returns>
        public virtual async Task<TaxSettingsModel> PrepareTaxSettingsModelAsync(TaxSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var taxSettings = await _settingService.LoadSettingAsync<TaxSettings>(storeId);

            //fill in model values from the entity
            model ??= taxSettings.ToSettingsModel<TaxSettingsModel>();
            model.TaxBasedOnValues = await taxSettings.TaxBasedOn.ToSelectListAsync();
            model.TaxDisplayTypeValues = await taxSettings.TaxDisplayType.ToSelectListAsync();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.PricesIncludeTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PricesIncludeTax, storeId);
                model.AllowUsersToSelectTaxDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.AllowUsersToSelectTaxDisplayType, storeId);
                model.TaxDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxDisplayType, storeId);
                model.DisplayTaxSuffix_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DisplayTaxSuffix, storeId);
                model.DisplayTaxRates_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DisplayTaxRates, storeId);
                model.HideZeroTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HideZeroTax, storeId);
                model.HideTaxInOrderSummary_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HideTaxInOrderSummary, storeId);
                model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, storeId);
                model.DefaultTaxCategoryId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxCategoryId, storeId);
                model.TaxBasedOn_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxBasedOn, storeId);
                model.TaxBasedOnPickupPointAddress_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxBasedOnPickupPointAddress, storeId);
                model.DefaultTaxAddress_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxAddressId, storeId);
                model.ShippingIsTaxable_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingIsTaxable, storeId);
                model.ShippingPriceIncludesTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingPriceIncludesTax, storeId);
                model.ShippingTaxClassId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingTaxClassId, storeId);
                model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeId);
                model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeId);
                model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeId);
                model.EuVatEnabled_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatEnabled, storeId);
                model.EuVatEnabledForGuests_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatEnabledForGuests, storeId);
                model.EuVatShopCountryId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatShopCountryId, storeId);
                model.EuVatAllowVatExemption_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatAllowVatExemption, storeId);
                model.EuVatUseWebService_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatUseWebService, storeId);
                model.EuVatAssumeValid_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatAssumeValid, storeId);
                model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeId);
            }

            //prepare available tax categories
            await _baseAdminModelFactory.PrepareTaxCategoriesAsync(model.TaxCategories);

            //prepare available EU VAT countries
            await _baseAdminModelFactory.PrepareCountriesAsync(model.EuVatShopCountries);

            //prepare default tax address
            var defaultAddress = await _addressService.GetAddressByIdAsync(taxSettings.DefaultTaxAddressId);
            if (defaultAddress != null)
                model.DefaultTaxAddress = defaultAddress.ToModel(model.DefaultTaxAddress);
            await _addressModelFactory.PrepareAddressModelAsync(model.DefaultTaxAddress, defaultAddress);
            model.DefaultTaxAddress.ZipPostalCodeRequired = true;

            return model;
        }

        /// <summary>
        /// Prepare catalog settings model
        /// </summary>
        /// <param name="model">Catalog settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the catalog settings model
        /// </returns>
        public virtual async Task<CatalogSettingsModel> PrepareCatalogSettingsModelAsync(CatalogSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeId);

            //fill in model values from the entity
            model ??= catalogSettings.ToSettingsModel<CatalogSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.AttributeValueOutOfStockDisplayTypes = await catalogSettings.AttributeValueOutOfStockDisplayType.ToSelectListAsync();
            model.TvChannelUrlStructureTypes = await ((TvChannelUrlStructureType)catalogSettings.TvChannelUrlStructureTypeId).ToSelectListAsync();
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.ViewMode.Grid"),
                Value = "grid"
            });
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.ViewMode.List"),
                Value = "list"
            });

            //fill in overridden values
            if (storeId > 0)
            {
                model.AllowViewUnpublishedTvChannelPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowViewUnpublishedTvChannelPage, storeId);
                model.DisplayDiscontinuedMessageForUnpublishedTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedTvChannels, storeId);
                model.ShowSkuOnTvChannelDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnTvChannelDetailsPage, storeId);
                model.ShowSkuOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnCatalogPages, storeId);
                model.ShowManufacturerPartNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowManufacturerPartNumber, storeId);
                model.ShowGtin_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowGtin, storeId);
                model.ShowFreeShippingNotification_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowFreeShippingNotification, storeId);
                model.ShowShortDescriptionOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowShortDescriptionOnCatalogPages, storeId);
                model.AllowTvChannelSorting_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowTvChannelSorting, storeId);
                model.AllowTvChannelViewModeChanging_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowTvChannelViewModeChanging, storeId);
                model.DefaultViewMode_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DefaultViewMode, storeId);
                model.ShowTvChannelsFromSubcategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowTvChannelsFromSubcategories, storeId);
                model.ShowCategoryTvChannelNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryTvChannelNumber, storeId);
                model.ShowCategoryTvChannelNumberIncludingSubcategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryTvChannelNumberIncludingSubcategories, storeId);
                model.CategoryBreadcrumbEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeId);
                model.ShowShareButton_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowShareButton, storeId);
                model.PageShareCode_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.PageShareCode, storeId);
                model.TvChannelReviewsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelReviewsMustBeApproved, storeId);
                model.OneReviewPerTvChannelFromUser_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.OneReviewPerTvChannelFromUser, storeId);
                model.AllowAnonymousUsersToReviewTvChannel_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToReviewTvChannel, storeId);
                model.TvChannelReviewPossibleOnlyAfterPurchasing_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelReviewPossibleOnlyAfterPurchasing, storeId);
                model.NotifyStoreOwnerAboutNewTvChannelReviews_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NotifyStoreOwnerAboutNewTvChannelReviews, storeId);
                model.NotifyUserAboutTvChannelReviewReply_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NotifyUserAboutTvChannelReviewReply, storeId);
                model.EmailAFriendEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EmailAFriendEnabled, storeId);
                model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeId);
                model.RecentlyViewedTvChannelsNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedTvChannelsNumber, storeId);
                model.RecentlyViewedTvChannelsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedTvChannelsEnabled, storeId);
                model.NewTvChannelsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewTvChannelsEnabled, storeId);
                model.NewTvChannelsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewTvChannelsPageSize, storeId);
                model.NewTvChannelsAllowUsersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewTvChannelsAllowUsersToSelectPageSize, storeId);
                model.NewTvChannelsPageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewTvChannelsPageSizeOptions, storeId);
                model.CompareTvChannelsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.CompareTvChannelsEnabled, storeId);
                model.ShowBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowBestsellersOnHomepage, storeId);
                model.NumberOfBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeId);
                model.SearchPageTvChannelsPerPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageTvChannelsPerPage, storeId);
                model.SearchPageAllowUsersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageAllowUsersToSelectPageSize, storeId);
                model.SearchPagePriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceRangeFiltering, storeId);
                model.SearchPagePriceFrom_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceFrom, storeId);
                model.SearchPagePriceTo_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceTo, storeId);
                model.SearchPageManuallyPriceRange_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageManuallyPriceRange, storeId);
                model.SearchPagePageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePageSizeOptions, storeId);
                model.TvChannelSearchAutoCompleteEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelSearchAutoCompleteEnabled, storeId);
                model.TvChannelSearchAutoCompleteNumberOfTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelSearchAutoCompleteNumberOfTvChannels, storeId);
                model.ShowTvChannelImagesInSearchAutoComplete_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowTvChannelImagesInSearchAutoComplete, storeId);
                model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowLinkToAllResultInSearchAutoComplete, storeId);
                model.TvChannelSearchTermMinimumLength_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelSearchTermMinimumLength, storeId);
                model.TvChannelsAlsoPurchasedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsAlsoPurchasedEnabled, storeId);
                model.TvChannelsAlsoPurchasedNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsAlsoPurchasedNumber, storeId);
                model.NumberOfTvChannelTags_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NumberOfTvChannelTags, storeId);
                model.TvChannelsByTagPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsByTagPageSize, storeId);
                model.TvChannelsByTagAllowUsersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsByTagAllowUsersToSelectPageSize, storeId);
                model.TvChannelsByTagPageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsByTagPageSizeOptions, storeId);
                model.TvChannelsByTagPriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsByTagPriceRangeFiltering, storeId);
                model.TvChannelsByTagPriceFrom_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsByTagPriceFrom, storeId);
                model.TvChannelsByTagPriceTo_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsByTagPriceTo, storeId);
                model.TvChannelsByTagManuallyPriceRange_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelsByTagManuallyPriceRange, storeId);
                model.IncludeShortDescriptionInCompareTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.IncludeShortDescriptionInCompareTvChannels, storeId);
                model.IncludeFullDescriptionInCompareTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.IncludeFullDescriptionInCompareTvChannels, storeId);
                model.ManufacturersBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeId);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeId);
                model.DisplayTaxShippingInfoTvChannelDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoTvChannelDetailsPage, storeId);
                model.DisplayTaxShippingInfoTvChannelBoxes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoTvChannelBoxes, storeId);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeId);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeId);
                model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeId);
                model.ShowTvChannelReviewsPerStore_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowTvChannelReviewsPerStore, storeId);
                model.ShowTvChannelReviewsOnAccountPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowTvChannelReviewsTabOnAccountPage, storeId);
                model.TvChannelReviewsPageSizeOnAccountPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelReviewsPageSizeOnAccountPage, storeId);
                model.TvChannelReviewsSortByCreatedDateAscending_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelReviewsSortByCreatedDateAscending, storeId);
                model.ExportImportTvChannelAttributes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportTvChannelAttributes, storeId);
                model.ExportImportTvChannelSpecificationAttributes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportTvChannelSpecificationAttributes, storeId);
                model.ExportImportTvChannelCategoryBreadcrumb_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportTvChannelCategoryBreadcrumb, storeId);
                model.ExportImportCategoriesUsingCategoryName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportCategoriesUsingCategoryName, storeId);
                model.ExportImportAllowDownloadImages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportAllowDownloadImages, storeId);
                model.ExportImportSplitTvChannelsFile_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportSplitTvChannelsFile, storeId);
                model.RemoveRequiredTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RemoveRequiredTvChannels, storeId);
                model.ExportImportRelatedEntitiesByName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportRelatedEntitiesByName, storeId);
                model.ExportImportTvChannelUseLimitedToStores_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportTvChannelUseLimitedToStores, storeId);
                model.DisplayDatePreOrderAvailability_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayDatePreOrderAvailability, storeId);
                model.UseAjaxCatalogTvChannelsLoading_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.UseAjaxCatalogTvChannelsLoading, storeId);
                model.EnableManufacturerFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnableManufacturerFiltering, storeId);
                model.EnablePriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnablePriceRangeFiltering, storeId);
                model.EnableSpecificationAttributeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnableSpecificationAttributeFiltering, storeId);
                model.DisplayFromPrices_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayFromPrices, storeId);
                model.AttributeValueOutOfStockDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AttributeValueOutOfStockDisplayType, storeId);
                model.AllowUsersToSearchWithManufacturerName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowUsersToSearchWithManufacturerName, storeId);
                model.AllowUsersToSearchWithCategoryName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowUsersToSearchWithCategoryName, storeId);
                model.DisplayAllPicturesOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayAllPicturesOnCatalogPages, storeId);
                model.TvChannelUrlStructureTypeId_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.TvChannelUrlStructureTypeId, storeId);
            }

            //prepare nested search model
            await PrepareSortOptionSearchModelAsync(model.SortOptionSearchModel);
            await _reviewTypeModelFactory.PrepareReviewTypeSearchModelAsync(model.ReviewTypeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sort option list model
        /// </returns>
        public virtual async Task<SortOptionListModel> PrepareSortOptionListModelAsync(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeId);

            //get sort options
            var sortOptions = Enum.GetValues(typeof(TvChannelSortingEnum)).OfType<TvChannelSortingEnum>().ToList().ToPagedList(searchModel);

            //prepare list model
            var model = await new SortOptionListModel().PrepareToGridAsync(searchModel, sortOptions, () =>
            {
                return sortOptions.SelectAwait(async option =>
                {
                    //fill in model values from the entity
                    var sortOptionModel = new SortOptionModel { Id = (int)option };

                    //fill in additional values (not existing in the entity)
                    sortOptionModel.Name = await _localizationService.GetLocalizedEnumAsync(option);
                    sortOptionModel.IsActive = !catalogSettings.TvChannelSortingEnumDisabled.Contains((int)option);
                    sortOptionModel.DisplayOrder = catalogSettings
                        .TvChannelSortingEnumDisplayOrder.TryGetValue((int)option, out var value) ? value : (int)option;

                    return sortOptionModel;
                }).OrderBy(option => option.DisplayOrder);
            });

            return model;
        }

        /// <summary>
        /// Prepare reward points settings model
        /// </summary>
        /// <param name="model">Reward points settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the reward points settings model
        /// </returns>
        public virtual async Task<RewardPointsSettingsModel> PrepareRewardPointsSettingsModelAsync(RewardPointsSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var rewardPointsSettings = await _settingService.LoadSettingAsync<RewardPointsSettings>(storeId);

            //fill in model values from the entity
            model ??= rewardPointsSettings.ToSettingsModel<RewardPointsSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.ActivatePointsImmediately = model.ActivationDelay <= 0;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.Enabled, storeId);
            model.ExchangeRate_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.ExchangeRate, storeId);
            model.MinimumRewardPointsToUse_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeId);
            model.MaximumRewardPointsToUsePerOrder_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MaximumRewardPointsToUsePerOrder, storeId);
            model.MaximumRedeemedRate_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MaximumRedeemedRate, storeId);
            model.PointsForRegistration_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForRegistration, storeId);
            model.RegistrationPointsValidity_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.RegistrationPointsValidity, storeId);
            model.PointsForPurchases_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeId) || await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForPurchases_Points, storeId);
            model.MinOrderTotalToAwardPoints_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MinOrderTotalToAwardPoints, storeId);
            model.PurchasesPointsValidity_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PurchasesPointsValidity, storeId);
            model.ActivationDelay_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.ActivationDelay, storeId);
            model.DisplayHowMuchWillBeEarned_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeId);
            model.PageSize_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PageSize, storeId);

            return model;
        }

        /// <summary>
        /// Prepare order settings model
        /// </summary>
        /// <param name="model">Order settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order settings model
        /// </returns>
        public virtual async Task<OrderSettingsModel> PrepareOrderSettingsModelAsync(OrderSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var orderSettings = await _settingService.LoadSettingAsync<OrderSettings>(storeId);

            //fill in model values from the entity
            model ??= orderSettings.ToSettingsModel<OrderSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.OrderIdent = await _dataProvider.GetTableIdentAsync<Order>();

            //fill in overridden values
            if (storeId > 0)
            {
                model.IsReOrderAllowed_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.IsReOrderAllowed, storeId);
                model.MinOrderSubtotalAmount_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderSubtotalAmount, storeId);
                model.MinOrderSubtotalAmountIncludingTax_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, storeId);
                model.MinOrderTotalAmount_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderTotalAmount, storeId);
                model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AutoUpdateOrderTotalsOnEditingOrder, storeId);
                model.AnonymousCheckoutAllowed_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AnonymousCheckoutAllowed, storeId);
                model.CheckoutDisabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.CheckoutDisabled, storeId);
                model.TermsOfServiceOnShoppingCartPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, storeId);
                model.TermsOfServiceOnOrderConfirmPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, storeId);
                model.OnePageCheckoutEnabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.OnePageCheckoutEnabled, storeId);
                model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, storeId);
                model.DisableBillingAddressCheckoutStep_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisableBillingAddressCheckoutStep, storeId);
                model.DisableOrderCompletedPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisableOrderCompletedPage, storeId);
                model.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisplayPickupInStoreOnShippingMethodPage, storeId);
                model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, storeId);
                model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, storeId);
                model.AttachPdfInvoiceToOrderProcessingEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderProcessingEmail, storeId);
                model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, storeId);
                model.ReturnRequestsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestsEnabled, storeId);
                model.ReturnRequestsAllowFiles_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestsAllowFiles, storeId);
                model.ReturnRequestNumberMask_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestNumberMask, storeId);
                model.NumberOfDaysReturnRequestAvailable_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeId);
                model.CustomOrderNumberMask_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.CustomOrderNumberMask, storeId);
                model.ExportWithTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ExportWithTvChannels, storeId);
                model.AllowAdminsToBuyCallForPriceTvChannels_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AllowAdminsToBuyCallForPriceTvChannels, storeId);
                model.ShowTvChannelThumbnailInOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ShowTvChannelThumbnailInOrderDetailsPage, storeId);
                model.DeleteGiftCardUsageHistory_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DeleteGiftCardUsageHistory, storeId);
            }

            //prepare nested search models
            await _returnRequestModelFactory.PrepareReturnRequestReasonSearchModelAsync(model.ReturnRequestReasonSearchModel);
            await _returnRequestModelFactory.PrepareReturnRequestActionSearchModelAsync(model.ReturnRequestActionSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare shopping cart settings model
        /// </summary>
        /// <param name="model">Shopping cart settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart settings model
        /// </returns>
        public virtual async Task<ShoppingCartSettingsModel> PrepareShoppingCartSettingsModelAsync(ShoppingCartSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var shoppingCartSettings = await _settingService.LoadSettingAsync<ShoppingCartSettings>(storeId);

            //fill in model values from the entity
            model ??= shoppingCartSettings.ToSettingsModel<ShoppingCartSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplayCartAfterAddingTvChannel_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.DisplayCartAfterAddingTvChannel, storeId);
            model.DisplayWishlistAfterAddingTvChannel_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.DisplayWishlistAfterAddingTvChannel, storeId);
            model.MaximumShoppingCartItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeId);
            model.MaximumWishlistItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumWishlistItems, storeId);
            model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeId);
            model.MoveItemsFromWishlistToCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeId);
            model.CartsSharedBetweenStores_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.CartsSharedBetweenStores, storeId);
            model.ShowTvChannelImagesOnShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowTvChannelImagesOnShoppingCart, storeId);
            model.ShowTvChannelImagesOnWishList_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowTvChannelImagesOnWishList, storeId);
            model.ShowDiscountBox_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowDiscountBox, storeId);
            model.ShowGiftCardBox_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowGiftCardBox, storeId);
            model.CrossSellsNumber_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.CrossSellsNumber, storeId);
            model.EmailWishlistEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.EmailWishlistEnabled, storeId);
            model.AllowAnonymousUsersToEmailWishlist_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeId);
            model.MiniShoppingCartEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeId);
            model.ShowTvChannelImagesInMiniShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowTvChannelImagesInMiniShoppingCart, storeId);
            model.MiniShoppingCartTvChannelNumber_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MiniShoppingCartTvChannelNumber, storeId);
            model.AllowCartItemEditing_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowCartItemEditing, storeId);
            model.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.GroupTierPricesForDistinctShoppingCartItems, storeId);

            return model;
        }

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <param name="model">Media settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the media settings model
        /// </returns>
        public virtual async Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>(storeId);

            //fill in model values from the entity
            model ??= mediaSettings.ToSettingsModel<MediaSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PicturesStoredIntoDatabase = await _pictureService.IsStoreInDbAsync();

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.AvatarPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AvatarPictureSize, storeId);
            model.TvChannelThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.TvChannelThumbPictureSize, storeId);
            model.TvChannelDetailsPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.TvChannelDetailsPictureSize, storeId);
            model.TvChannelThumbPictureSizeOnTvChannelDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.TvChannelThumbPictureSizeOnTvChannelDetailsPage, storeId);
            model.AssociatedTvChannelPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AssociatedTvChannelPictureSize, storeId);
            model.CategoryThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CategoryThumbPictureSize, storeId);
            model.ManufacturerThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ManufacturerThumbPictureSize, storeId);
            model.VendorThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.VendorThumbPictureSize, storeId);
            model.CartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CartThumbPictureSize, storeId);
            model.OrderThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.OrderThumbPictureSize, storeId);
            model.MiniCartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MiniCartThumbPictureSize, storeId);
            model.MaximumImageSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MaximumImageSize, storeId);
            model.MultipleThumbDirectories_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MultipleThumbDirectories, storeId);
            model.DefaultImageQuality_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultImageQuality, storeId);
            model.ImportTvChannelImagesUsingHash_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ImportTvChannelImagesUsingHash, storeId);
            model.DefaultPictureZoomEnabled_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultPictureZoomEnabled, storeId);
            model.AllowSVGUploads_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AllowSVGUploads, storeId);
            model.TvChannelDefaultImageId_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.TvChannelDefaultImageId, storeId);

            return model;
        }

        /// <summary>
        /// Prepare user user settings model
        /// </summary>
        /// <param name="model">User user settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user user settings model
        /// </returns>
        public virtual async Task<UserUserSettingsModel> PrepareUserUserSettingsModelAsync(UserUserSettingsModel model = null)
        {
            model ??= new UserUserSettingsModel
            {
                ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
            };

            //prepare user settings model
            model.UserSettings = await PrepareUserSettingsModelAsync();

            //prepare multi-factor authentication settings model
            model.MultiFactorAuthenticationSettings = await PrepareMultiFactorAuthenticationSettingsModelAsync();

            //prepare address settings model
            model.AddressSettings = await PrepareAddressSettingsModelAsync();

            //prepare date time settings model
            model.DateTimeSettings = await PrepareDateTimeSettingsModelAsync();

            //prepare external authentication settings model
            model.ExternalAuthenticationSettings = await PrepareExternalAuthenticationSettingsModelAsync();

            //prepare nested search models
            await _userAttributeModelFactory.PrepareUserAttributeSearchModelAsync(model.UserAttributeSearchModel);
            await _addressAttributeModelFactory.PrepareAddressAttributeSearchModelAsync(model.AddressAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare GDPR settings model
        /// </summary>
        /// <param name="model">Gdpr settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR settings model
        /// </returns>
        public virtual async Task<GdprSettingsModel> PrepareGdprSettingsModelAsync(GdprSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var gdprSettings = await _settingService.LoadSettingAsync<GdprSettings>(storeId);

            //fill in model values from the entity
            model ??= gdprSettings.ToSettingsModel<GdprSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //prepare nested search model
            await PrepareGdprConsentSearchModelAsync(model.GdprConsentSearchModel);

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.GdprEnabled_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.GdprEnabled, storeId);
            model.LogPrivacyPolicyConsent_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogPrivacyPolicyConsent, storeId);
            model.LogNewsletterConsent_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogNewsletterConsent, storeId);
            model.LogUserProfileChanges_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogUserProfileChanges, storeId);
            model.DeleteInactiveUsersAfterMonths_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.DeleteInactiveUsersAfterMonths, storeId);

            return model;
        }

        /// <summary>
        /// Prepare paged GDPR consent list model
        /// </summary>
        /// <param name="searchModel">GDPR search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent list model
        /// </returns>
        public virtual async Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get sort options
            var consentList = (await _gdprService.GetAllConsentsAsync()).ToPagedList(searchModel);

            //prepare list model
            var model = await new GdprConsentListModel().PrepareToGridAsync(searchModel, consentList, () =>
            {
                return consentList.SelectAwait(async consent =>
                {
                    var gdprConsentModel = consent.ToModel<GdprConsentModel>();

                    var gdprConsent = await _gdprService.GetConsentByIdAsync(gdprConsentModel.Id);
                    gdprConsentModel.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message);
                    gdprConsentModel.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage);

                    return gdprConsentModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare GDPR consent model
        /// </summary>
        /// <param name="model">GDPR consent model</param>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent model
        /// </returns>
        public virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false)
        {
            Func<GdprConsentLocalizedModel, int, Task> localizedModelConfiguration = null;

            //fill in model values from the entity
            if (gdprConsent != null)
            {
                model ??= gdprConsent.ToModel<GdprConsentModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message, languageId, false, false);
                    locale.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage, languageId, false, false);
                };
            }

            //set default values for the new model
            if (gdprConsent == null)
                model.DisplayOrder = 1;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <param name="model">General common settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the general and common settings model
        /// </returns>
        public virtual async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null)
        {
            model ??= new GeneralCommonSettingsModel
            {
                ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
            };

            //prepare store information settings model
            model.StoreInformationSettings = await PrepareStoreInformationSettingsModelAsync();

            //prepare Sitemap settings model
            model.SitemapSettings = await PrepareSitemapSettingsModelAsync();

            //prepare Minification settings model
            model.MinificationSettings = await PrepareMinificationSettingsModelAsync();

            //prepare SEO settings model
            model.SeoSettings = await PrepareSeoSettingsModelAsync();

            //prepare security settings model
            model.SecuritySettings = await PrepareSecuritySettingsModelAsync();

            //prepare robots.txt settings model
            model.RobotsTxtSettings = await PrepareRobotsTxtSettingsModelAsync();

            //prepare captcha settings model
            model.CaptchaSettings = await PrepareCaptchaSettingsModelAsync();

            //prepare PDF settings model
            model.PdfSettings = await PreparePdfSettingsModelAsync();

            //prepare localization settings model
            model.LocalizationSettings = await PrepareLocalizationSettingsModelAsync();

            //prepare admin area settings model
            model.AdminAreaSettings = await PrepareAdminAreaSettingsModelAsync();

            //prepare display default menu item settings model
            model.DisplayDefaultMenuItemSettings = await PrepareDisplayDefaultMenuItemSettingsModelAsync();

            //prepare display default footer item settings model
            model.DisplayDefaultFooterItemSettings = await PrepareDisplayDefaultFooterItemSettingsModelAsync();

            //prepare custom HTML settings model
            model.CustomHtmlSettings = await PrepareCustomHtmlSettingsModelAsync();

            return model;
        }

        /// <summary>
        /// Prepare tvchannel editor settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel editor settings model
        /// </returns>
        public virtual async Task<TvChannelEditorSettingsModel> PrepareTvChannelEditorSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var tvchannelEditorSettings = await _settingService.LoadSettingAsync<TvChannelEditorSettings>(storeId);

            //fill in model values from the entity
            var model = tvchannelEditorSettings.ToSettingsModel<TvChannelEditorSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting search model
        /// </returns>
        public virtual async Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare model to add
            await PrepareAddSettingModelAsync(searchModel.AddSetting);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting list model
        /// </returns>
        public virtual async Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get settings
            var settings = (await _settingService.GetAllSettingsAsync()).AsQueryable();

            //filter settings
            if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
                settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
                settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

            var pagedSettings = settings.ToList().ToPagedList(searchModel);

            //prepare list model
            var model = await new SettingListModel().PrepareToGridAsync(searchModel, pagedSettings, () =>
            {
                return pagedSettings.SelectAwait(async setting =>
                {
                    //fill in model values from the entity
                    var settingModel = setting.ToModel<SettingModel>();

                    //fill in additional values (not existing in the entity)
                    settingModel.Store = setting.StoreId > 0
                        ? (await _storeService.GetStoreByIdAsync(setting.StoreId))?.Name ?? "Deleted"
                        : await _localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");

                    return settingModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting mode model
        /// </returns>
        public virtual async Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName)
        {
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), modeName)
            };

            return model;
        }

        /// <summary>
        /// Prepare store scope configuration model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store scope configuration model
        /// </returns>
        public virtual async Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync()
        {
            var model = new StoreScopeConfigurationModel
            {
                Stores = (await _storeService.GetAllStoresAsync()).Select(store => store.ToModel<StoreModel>()).ToList(),
                StoreId = await _storeContext.GetActiveStoreScopeConfigurationAsync()
            };

            return model;
        }

        #endregion
    }
}
