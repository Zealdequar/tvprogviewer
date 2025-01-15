using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Forums;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Themes;
using TvProgViewer.Services.Topics;
using TvProgViewer.Services.TvProgMain;
using TvProgViewer.Web.Framework.Themes;
using TvProgViewer.Web.Framework.UI;
using TvProgViewer.WebUI.Infrastructure.Cache;
using TvProgViewer.WebUI.Models.Common;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the common models factory
    /// </summary>
    public partial class CommonModelFactory : ICommonModelFactory
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly UserSettings _userSettings;
        private readonly DisplayDefaultFooterItemSettings _displayDefaultFooterItemSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly ITvProgHtmlHelper _nopHtmlHelper;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly RobotsTxtSettings _robotsTxtSettings;
        private readonly SitemapSettings _sitemapSettings;
        private readonly SitemapXmlSettings _sitemapXmlSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IProgrammeService _programmeService;
        private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;

        #endregion

        #region Ctor

        public CommonModelFactory(BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            UserSettings userSettings,
            DisplayDefaultFooterItemSettings displayDefaultFooterItemSettings,
            ForumSettings forumSettings,
            ICurrencyService currencyService,
            IUserService userService,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ITvProgFileProvider fileProvider,
            ITvProgHtmlHelper nopHtmlHelper,
            IPermissionService permissionService,
            IPictureService pictureService,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            NewsSettings newsSettings,
            RobotsTxtSettings robotsTxtSettings,
            SitemapSettings sitemapSettings,
            SitemapXmlSettings sitemapXmlSettings,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings,
            IProgrammeService programmeService,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings)
        {
            _blogSettings = blogSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _userSettings = userSettings;
            _displayDefaultFooterItemSettings = displayDefaultFooterItemSettings;
            _forumSettings = forumSettings;
            _currencyService = currencyService;
            _userService = userService;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _nopHtmlHelper = nopHtmlHelper;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _shoppingCartService = shoppingCartService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _themeProvider = themeProvider;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _localizationSettings = localizationSettings;
            _newsSettings = newsSettings;
            _robotsTxtSettings = robotsTxtSettings;
            _sitemapSettings = sitemapSettings;
            _sitemapXmlSettings = sitemapXmlSettings;
            _storeInformationSettings = storeInformationSettings;
            _vendorSettings = vendorSettings;
            _programmeService = programmeService;
            _displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the number of unread private messages
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of private messages
        /// </returns>
        protected virtual async Task<int> GetUnreadPrivateMessagesAsync()
        {
            var result = 0;
            var user = await _workContext.GetCurrentUserAsync();
            if (_forumSettings.AllowPrivateMessages && !await _userService.IsGuestAsync(user))
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var privateMessages = await _forumService.GetAllPrivateMessagesAsync(store.Id,
                    0, user.Id, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = privateMessages.TotalCount;
                }
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the logo model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the logo model
        /// </returns>
        public virtual async Task<LogoModel> PrepareLogoModelAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var model = new LogoModel
            {
                StoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name)
            };

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.StoreLogoPath
                , store, await _themeContext.GetWorkingThemeNameAsync(), _webHelper.IsCurrentConnectionSecured());
            model.LogoPath = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var logo = string.Empty;
                var logoPictureId = _storeInformationSettings.LogoPictureId;

                if (logoPictureId > 0)
                    logo = await _pictureService.GetPictureUrlAsync(logoPictureId, showDefaultPicture: false);

                if (string.IsNullOrEmpty(logo))
                {
                    //use default logo
                    var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
                    var storeLocation = _mediaSettings.UseAbsoluteImagePath ? _webHelper.GetStoreLocation() : $"{pathBase}/";
                    logo = $"{storeLocation}Themes/{await _themeContext.GetWorkingThemeNameAsync()}/Content/images/logo.png";
                }

                return logo;
            });

            return model;
        }

        /// <summary>
        /// Prepare the language selector model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the language selector model
        /// </returns>
        public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var availableLanguages = (await _languageService
                    .GetAllLanguagesAsync(storeId: store.Id))
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    }).ToList();

            var model = new LanguageSelectorModel
            {
                CurrentLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            return model;
        }
        
        /// <summary>
        /// Подготовка модели для выбора категории ТВ-программы
        /// </summary>
        /// <returns>Модель для выбора категории ТВ-программы</returns>
        public virtual async Task<TvCategorySelectorModel> PrepareTvCategorySelectorModelAsync()
        {
            var avaliableCategories = (await _programmeService
                .GetCategoriesAsync())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                {
                    var categoryModel = new TvCategoryModel
                    {
                        Name = x
                    };
                    return categoryModel;
                }).ToList();
            avaliableCategories.Insert(0, new TvCategoryModel { Name = "Все категории" });

            var model = new TvCategorySelectorModel
            {
                CurrentCategoryName = await _workContext.GetWorkingCategoryAsync(),
                AvaliableCategories = avaliableCategories
            };

            return model;
        }

        /// <summary>
        /// Prepare the currency selector model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the currency selector model
        /// </returns>
        public virtual async Task<CurrencySelectorModel> PrepareCurrencySelectorModelAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var availableCurrencies = await (await _currencyService
                .GetAllCurrenciesAsync(storeId: store.Id))
                .SelectAwait(async x =>
                {
                    //currency char
                    var currencySymbol = !string.IsNullOrEmpty(x.DisplayLocale)
                        ? new RegionInfo(x.DisplayLocale).CurrencySymbol
                        : x.CurrencyCode;

                    //model
                    var currencyModel = new CurrencyModel
                    {
                        Id = x.Id,
                        Name = await _localizationService.GetLocalizedAsync(x, y => y.Name),
                        CurrencySymbol = currencySymbol
                    };

                    return currencyModel;
                }).ToListAsync();

            var model = new CurrencySelectorModel
            {
                CurrentCurrencyId = (await _workContext.GetWorkingCurrencyAsync()).Id,
                AvailableCurrencies = availableCurrencies
            };

            return model;
        }

        /// <summary>
        /// Prepare the tax type selector model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax type selector model
        /// </returns>
        public virtual async Task<TaxTypeSelectorModel> PrepareTaxTypeSelectorModelAsync()
        {
            var model = new TaxTypeSelectorModel
            {
                CurrentTaxType = await _workContext.GetTaxDisplayTypeAsync()
            };

            return model;
        }

        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the header links model
        /// </returns>
        public virtual async Task<HeaderLinksModel> PrepareHeaderLinksModelAsync()
        {
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            var unreadMessageCount = await GetUnreadPrivateMessagesAsync();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = string.Format(await _localizationService.GetResourceAsync("PrivateMessages.TotalUnread"), unreadMessageCount);

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.NotifiedAboutNewPrivateMessagesAttribute, store.Id))
                {
                    await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.NotifiedAboutNewPrivateMessagesAttribute, true, store.Id);
                    alertMessage = string.Format(await _localizationService.GetResourceAsync("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }
            // Топики меню
            var topicModel = await (await _topicService.GetAllTopicsAsync(store.Id, onlyIncludedInTopMenu: true))
                .SelectAwait(async t => new HeaderLinksModel.TopicModel
                {
                    Id = t.Id,
                    Name = await _localizationService.GetLocalizedAsync(t, x => x.Title),
                    SeName = await _urlRecordService.GetSeNameAsync(t)
                }).ToListAsync();

            var model = new HeaderLinksModel
            {
                RegistrationType = _userSettings.UserRegistrationType,
                IsAuthenticated = await _userService.IsRegisteredAsync(user),
                UserName = await _userService.IsRegisteredAsync(user) ? await _userService.FormatUsernameAsync(user) : string.Empty,
                ShoppingCartEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart),
                Topics = topicModel,
                DisplayHomepageMenuItem = _displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
                DisplayTvChannelSearchMenuItem = _displayDefaultMenuItemSettings.DisplayTvChannelSearchMenuItem,
                DisplayContactUsMenuItem = _displayDefaultMenuItemSettings.DisplayContactUsMenuItem,
                WishlistEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist),
                AllowPrivateMessages = await _userService.IsRegisteredAsync(user) && _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };
            //performance optimization (use "HasShoppingCartItems" property)
            if (user.HasShoppingCartItems)
            {
                model.ShoppingCartItems = (await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id))
                    .Sum(item => item.Quantity);

                model.WishlistItems = (await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.Wishlist, store.Id))
                    .Sum(item => item.Quantity);
            }

            return model;
        }

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the admin header links model
        /// </returns>
        public virtual async Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModelAsync()
        {
            var user = await _workContext.GetCurrentUserAsync();

            var model = new AdminHeaderLinksModel
            {
                ImpersonatedUserName = await _userService.IsRegisteredAsync(user) ? await _userService.FormatUsernameAsync(user) : string.Empty,
                IsUserImpersonated = _workContext.OriginalUserIfImpersonated != null,
                DisplayAdminLink = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel),
                EditPageUrl = _nopHtmlHelper.GetEditPageUrl()
            };

            return model;
        }

        /// <summary>
        /// Prepare the social model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the social model
        /// </returns>
        public virtual async Task<SocialModel> PrepareSocialModelAsync()
        {
            var model = new SocialModel
            {
                FacebookLink = _storeInformationSettings.FacebookLink,
                TwitterLink = _storeInformationSettings.TwitterLink,
                YoutubeLink = _storeInformationSettings.YoutubeLink,
                InstagramLink = _storeInformationSettings.InstagramLink,
                WorkingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
                NewsEnabled = _newsSettings.Enabled,
            };

            return model;
        }

        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the footer model
        /// </returns>
        public virtual async Task<FooterModel> PrepareFooterModelAsync()
        {
            //footer topics
            var store = await _storeContext.GetCurrentStoreAsync();
            var topicModels = await (await _topicService.GetAllTopicsAsync(store.Id))
                    .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
                    .SelectAwait(async t => new FooterModel.FooterTopicModel
                    {
                        Id = t.Id,
                        Name = await _localizationService.GetLocalizedAsync(t, x => x.Title),
                        SeName = await _urlRecordService.GetSeNameAsync(t),
                        IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                        IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                        IncludeInFooterColumn3 = t.IncludeInFooterColumn3
                    }).ToListAsync();

            //model
            var model = new FooterModel
            {
                StoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name),
                WishlistEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist),
                ShoppingCartEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart),
                SitemapEnabled = _sitemapSettings.SitemapEnabled,
                SearchEnabled = _catalogSettings.TvChannelSearchEnabled,
                WorkingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
                BlogEnabled = _blogSettings.Enabled,
                CompareTvChannelsEnabled = _catalogSettings.CompareTvChannelsEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                NewsEnabled = _newsSettings.Enabled,
                RecentlyViewedTvChannelsEnabled = _catalogSettings.RecentlyViewedTvChannelsEnabled,
                NewTvChannelsEnabled = _catalogSettings.NewTvChannelsEnabled,
                DisplayTaxShippingInfoFooter = _catalogSettings.DisplayTaxShippingInfoFooter,
                HidePoweredByNopCommerce = _storeInformationSettings.HidePoweredByNopCommerce,
                IsHomePage = _webHelper.GetStoreLocation().Equals(_webHelper.GetThisPageUrl(false), StringComparison.InvariantCultureIgnoreCase),
                AllowUsersToApplyForVendorAccount = _vendorSettings.AllowUsersToApplyForVendorAccount,
                AllowUsersToCheckGiftCardBalance = _userSettings.AllowUsersToCheckGiftCardBalance && _captchaSettings.Enabled,
                Topics = topicModels,
                DisplaySitemapFooterItem = _displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = _displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayTvChannelSearchFooterItem = _displayDefaultFooterItemSettings.DisplayTvChannelSearchFooterItem,
                DisplayNewsFooterItem = _displayDefaultFooterItemSettings.DisplayNewsFooterItem,
                DisplayBlogFooterItem = _displayDefaultFooterItemSettings.DisplayBlogFooterItem,
                DisplayForumsFooterItem = _displayDefaultFooterItemSettings.DisplayForumsFooterItem,
                DisplayRecentlyViewedTvChannelsFooterItem = _displayDefaultFooterItemSettings.DisplayRecentlyViewedTvChannelsFooterItem,
                DisplayCompareTvChannelsFooterItem = _displayDefaultFooterItemSettings.DisplayCompareTvChannelsFooterItem,
                DisplayNewTvChannelsFooterItem = _displayDefaultFooterItemSettings.DisplayNewTvChannelsFooterItem,
                DisplayUserInfoFooterItem = _displayDefaultFooterItemSettings.DisplayUserInfoFooterItem,
                DisplayUserOrdersFooterItem = _displayDefaultFooterItemSettings.DisplayUserOrdersFooterItem,
                DisplayUserAddressesFooterItem = _displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem,
                DisplayShoppingCartFooterItem = _displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem,
                DisplayWishlistFooterItem = _displayDefaultFooterItemSettings.DisplayWishlistFooterItem,
                DisplayApplyVendorAccountFooterItem = _displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem
            };

            return model;
        }

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the contact us model
        /// </returns>
        public virtual async Task<ContactUsModel> PrepareContactUsModelAsync(ContactUsModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties)
            {
                var user = await _workContext.GetCurrentUserAsync();
                model.Email = user.Email;
                model.FullName = await _userService.GetUserFullNameAsync(user);
            }

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            model.MetaKeywords = await _localizationService.GetResourceAsync("PageTitle.ContactUs.MetaKeywords");
            model.MetaDescription = await _localizationService.GetResourceAsync("PageTitle.ContactUs.MetaDescription");

            return model;
        }

        /// <summary>
        /// Prepare the contact vendor model
        /// </summary>
        /// <param name="model">Contact vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the contact vendor model
        /// </returns>
        public virtual async Task<ContactVendorModel> PrepareContactVendorModelAsync(ContactVendorModel model, Vendor vendor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (!excludeProperties)
            {
                var user = await _workContext.GetCurrentUserAsync();
                model.Email = user.Email;
                model.FullName = await _userService.GetUserFullNameAsync(user);
            }

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            model.VendorId = vendor.Id;
            model.VendorName = await _localizationService.GetLocalizedAsync(vendor, x => x.Name);

            return model;
        }

        /// <summary>
        /// Prepare the store theme selector model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the store theme selector model
        /// </returns>
        public virtual async Task<StoreThemeSelectorModel> PrepareStoreThemeSelectorModelAsync()
        {
            var model = new StoreThemeSelectorModel();

            var currentTheme = await _themeProvider.GetThemeBySystemNameAsync(await _themeContext.GetWorkingThemeNameAsync());
            model.CurrentStoreTheme = new StoreThemeModel
            {
                Name = currentTheme?.SystemName,
                Title = currentTheme?.FriendlyName
            };

            model.AvailableStoreThemes = (await _themeProvider.GetThemesAsync()).Select(x => new StoreThemeModel
            {
                Name = x.SystemName,
                Title = x.FriendlyName
            }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the favicon model
        /// </returns>
        public virtual Task<FaviconAndAppIconsModel> PrepareFaviconAndAppIconsModelAsync()
        {
            var model = new FaviconAndAppIconsModel
            {
                HeadCode = _commonSettings.FaviconAndAppIconsHeadCode
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the robots.txt file as string
        /// </returns>
        public virtual async Task<string> PrepareRobotsTextFileAsync()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsCustomFileName);
            if (_fileProvider.FileExists(robotsFilePath))
            {
                //the robots.txt file exists
                var robotsFileContent = await _fileProvider.ReadAllTextAsync(robotsFilePath, Encoding.UTF8);
                sb.Append(robotsFileContent);
            }
            else
            {
                sb.AppendLine("User-agent: *");

                //sitemap
                if (_sitemapXmlSettings.SitemapXmlEnabled && _robotsTxtSettings.AllowSitemapXml) 
                    sb.AppendLine($"Sitemap: {_webHelper.GetStoreLocation()}sitemap.xml");
                else
                    sb.AppendLine("Disallow: /sitemap.xml");

                //host
                sb.AppendLine($"Host: {_webHelper.GetStoreLocation()}");

                //usual paths
                foreach (var path in _robotsTxtSettings.DisallowPaths) 
                    sb.AppendLine($"Disallow: {path}");

                //localizable paths (without SEO code)
                foreach (var path in _robotsTxtSettings.LocalizableDisallowPaths)
                    sb.AppendLine($"Disallow: {path}");

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    var store = await _storeContext.GetCurrentStoreAsync();
                    //URLs are localizable. Append SEO code
                    foreach (var language in await _languageService.GetAllLanguagesAsync(storeId: store.Id))
                        if (_robotsTxtSettings.DisallowLanguages.Contains(language.Id))
                            sb.AppendLine($"Disallow: /{language.UniqueSeoCode}*");
                        else
                            foreach (var path in _robotsTxtSettings.LocalizableDisallowPaths)
                                sb.AppendLine($"Disallow: /{language.UniqueSeoCode}{path}");
                }

                foreach (var additionsRule in _robotsTxtSettings.AdditionsRules)
                    sb.Append(additionsRule);

                //load and add robots.txt additions to the end of file.
                var robotsAdditionsFile = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsAdditionsFileName);
                if (_fileProvider.FileExists(robotsAdditionsFile))
                {
                    sb.AppendLine();
                    var robotsFileContent = await _fileProvider.ReadAllTextAsync(robotsAdditionsFile, Encoding.UTF8);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}