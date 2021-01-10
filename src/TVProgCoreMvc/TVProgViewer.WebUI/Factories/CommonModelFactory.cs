using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Services.Blogs;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Forums;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Media;
using TVProgViewer.Services.News;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Seo;
using TVProgViewer.Services.Themes;
using TVProgViewer.Services.Topics;
using TVProgViewer.Web.Framework.Themes;
using TVProgViewer.Web.Framework.UI;
using TVProgViewer.WebUI.Infrastructure.Cache;
using TVProgViewer.WebUI.Models.Common;
using TVProgViewer.Services.TvProgMain;

namespace TVProgViewer.WebUI.Factories
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
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IBlogService _blogService;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INewsService _newsService;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly ITopicService _topicService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly SitemapSettings _sitemapSettings;
        private readonly SitemapXmlSettings _sitemapXmlSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IProgrammeService _programmeService;

        #endregion

        #region Ctor

        public CommonModelFactory(BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            UserSettings userSettings,
            DisplayDefaultFooterItemSettings displayDefaultFooterItemSettings,
            ForumSettings forumSettings,
            IActionContextAccessor actionContextAccessor,
            IBlogService blogService,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            IUserService userService,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            INewsService newsService,
            ITvProgFileProvider fileProvider,
            IPageHeadBuilder pageHeadBuilder,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IProductTagService productTagService,
            IShoppingCartService shoppingCartService,
            ISitemapGenerator sitemapGenerator,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            ITopicService topicService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            NewsSettings newsSettings,
            SitemapSettings sitemapSettings,
            SitemapXmlSettings sitemapXmlSettings,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings,
            IProgrammeService programmeService)
        {
            _blogSettings = blogSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _userSettings = userSettings;
            _displayDefaultFooterItemSettings = displayDefaultFooterItemSettings;
            _forumSettings = forumSettings;
            _actionContextAccessor = actionContextAccessor;
            _blogService = blogService;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _userService = userService;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _newsService = newsService;
            _fileProvider = fileProvider;
            _pageHeadBuilder = pageHeadBuilder;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _productTagService = productTagService;
            _shoppingCartService = shoppingCartService;
            _sitemapGenerator = sitemapGenerator;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _themeProvider = themeProvider;
            _topicService = topicService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _localizationSettings = localizationSettings;
            _newsSettings = newsSettings;
            _sitemapSettings = sitemapSettings;
            _sitemapXmlSettings = sitemapXmlSettings;
            _storeInformationSettings = storeInformationSettings;
            _vendorSettings = vendorSettings;
            _programmeService = programmeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the number of unread private messages
        /// </summary>
        /// <returns>Number of private messages</returns>
        protected virtual int GetUnreadPrivateMessages()
        {
            var result = 0;
            var user = _workContext.CurrentUser;
            if (_forumSettings.AllowPrivateMessages && !_userService.IsGuest(user))
            {
                var privateMessages = _forumService.GetAllPrivateMessages(_storeContext.CurrentStore.Id,
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
        /// <returns>Logo model</returns>
        public virtual LogoModel PrepareLogoModel()
        {
            var model = new LogoModel
            {
                StoreName = _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name)
            };

            var cacheKey = TvProgModelCacheDefaults.StoreLogoPath.FillCacheKey(_storeContext.CurrentStore.Id, _themeContext.WorkingThemeName, _webHelper.IsCurrentConnectionSecured());
            model.LogoPath = _cacheManager.Get(cacheKey, () =>
            {
                var logo = string.Empty;
                var logoPictureId = _storeInformationSettings.LogoPictureId;

                if (logoPictureId > 0)
                    logo = _pictureService.GetPictureUrl(logoPictureId, showDefaultPicture: false);

                if (string.IsNullOrEmpty(logo))
                {
                    //use default logo
                    var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
                    var storeLocation = _mediaSettings.UseAbsoluteImagePath ? _webHelper.GetStoreLocation() : $"{pathBase}/";
                    logo = $"{storeLocation}Themes/{_themeContext.WorkingThemeName}/Content/images/logo.png";
                }

                return logo;
            });

            return model;
        }

        /// <summary>
        /// Prepare the language selector model
        /// </summary>
        /// <returns>Language selector model</returns>
        public virtual LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _languageService
                    .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    }).ToList();

            var model = new LanguageSelectorModel
            {
                CurrentLanguageId = _workContext.WorkingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            return model;
        }

        /// <summary>
        /// Подготовка модели для выбора провайдера ТВ-программы
        /// </summary>
        /// <returns>Модель для выбора провайдера ТВ-программы</returns>
        public virtual TvProgProviderSelectorModel PrepareTvProgProviderSelectorModel()
        {
            var avaliableProviders = _programmeService
                .GetAllProviders().Select(x =>
                {
                    var providerModel = new TvProgProviderModel
                    {
                        Id = x.Id,
                        Name = x.ProviderName
                    };
                    return providerModel;
                }).ToList();

            var model = new TvProgProviderSelectorModel
            {
                CurrentProviderId = _workContext.WorkingProvider.Id,
                AvaliableProviders = avaliableProviders
            };
            return model;
        }

        /// <summary>
        /// Подготовка модели для выбора типа ТВ-программы
        /// </summary>
        /// <returns>Модель для выбора типа ТВ-программы</returns>
        public virtual TvTypeProgSelectorModel PrepareTvTypeProgSelectorModel()
        {
            var avaliableTypes = _programmeService
                .GetAllTypeProgs()
                .Where(x => x.TvProgProviderId == _workContext.WorkingProvider.Id)
                .Select(x =>
                {
                    var typeModel = new TvTypeProgModel
                    {
                        Id = x.Id,
                        Name = x.TypeName
                    };
                    return typeModel;
                }).ToList();

            var model = new TvTypeProgSelectorModel
            {
                CurrentTypeProgId = _workContext.WorkingTypeProg.Id,
                AvaliableTypes = avaliableTypes
            };

            return model;
        }

        /// <summary>
        /// Подготовка модели для выбора категории ТВ-программы
        /// </summary>
        /// <returns>Модель для выбора категории ТВ-программы</returns>
        public virtual TvCategorySelectorModel PrepareTvCategorySelectorModel()
        {
            var avaliableCategories = _programmeService
                .GetCategories()
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
                CurrentCategoryName = _workContext.WorkingCategory,
                AvaliableCategories = avaliableCategories
            };

            return model;
        }

        /// <summary>
        /// Prepare the tax type selector model
        /// </summary>
        /// <returns>Tax type selector model</returns>
        public virtual TaxTypeSelectorModel PrepareTaxTypeSelectorModel()
        {
            var model = new TaxTypeSelectorModel
            {
                CurrentTaxType = _workContext.TaxDisplayType
            };

            return model;
        }

        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>Header links model</returns>
        public virtual HeaderLinksModel PrepareHeaderLinksModel()
        {
            var user = _workContext.CurrentUser;

            var unreadMessageCount = GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = string.Format(_localizationService.GetResource("PrivateMessages.TotalUnread"), unreadMessageCount);

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !_genericAttributeService.GetAttribute<bool>(user, TvProgUserDefaults.NotifiedAboutNewPrivateMessagesAttribute, _storeContext.CurrentStore.Id))
                {
                    _genericAttributeService.SaveAttribute(user, TvProgUserDefaults.NotifiedAboutNewPrivateMessagesAttribute, true, _storeContext.CurrentStore.Id);
                    alertMessage = string.Format(_localizationService.GetResource("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            var model = new HeaderLinksModel
            {
                IsAuthenticated = _userService.IsRegistered(user),
                UserName = _userService.IsRegistered(user) ? _userService.FormatUsername(user) : string.Empty,
                ShoppingCartEnabled = false,
                WishlistEnabled = false, //_permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                AllowPrivateMessages = _userService.IsRegistered(user) && _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };
            //performance optimization (use "HasShoppingCartItems" property)
            if (user.HasShoppingCartItems)
            {
                model.ShoppingCartItems = _shoppingCartService.GetShoppingCart(user, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id)
                    .Sum(item => item.Quantity);

                model.WishlistItems = _shoppingCartService.GetShoppingCart(user, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id)
                    .Sum(item => item.Quantity);
            }

            return model;
        }

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        public virtual AdminHeaderLinksModel PrepareAdminHeaderLinksModel()
        {
            var user = _workContext.CurrentUser;

            var model = new AdminHeaderLinksModel
            {
                ImpersonatedUserName = _userService.IsRegistered(user) ? _userService.FormatUsername(user) : string.Empty,
                IsUserImpersonated = _workContext.OriginalUserIfImpersonated != null,
                DisplayAdminLink = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
                EditPageUrl = _pageHeadBuilder.GetEditPageUrl()
            };

            return model;
        }

        /// <summary>
        /// Prepare the social model
        /// </summary>
        /// <returns>Social model</returns>
        public virtual SocialModel PrepareSocialModel()
        {
            var model = new SocialModel
            {
                FacebookLink = _storeInformationSettings.FacebookLink,
                TwitterLink = _storeInformationSettings.TwitterLink,
                YoutubeLink = _storeInformationSettings.YoutubeLink,
                WorkingLanguageId = _workContext.WorkingLanguage.Id,
                NewsEnabled = _newsSettings.Enabled,
            };

            return model;
        }

        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>Footer model</returns>
        public virtual FooterModel PrepareFooterModel()
        {
            //footer topics
            var topicModels = _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                    .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
                    .Select(t => new FooterModel.FooterTopicModel
                    {
                        Id = t.Id,
                        Name = _localizationService.GetLocalized(t, x => x.Title),
                        SeName = _urlRecordService.GetSeName(t),
                        IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                        IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                        IncludeInFooterColumn3 = t.IncludeInFooterColumn3
                    }).ToList();

            //model
            var model = new FooterModel
            {
                StoreName = _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                SitemapEnabled = _sitemapSettings.SitemapEnabled,
                WorkingLanguageId = _workContext.WorkingLanguage.Id,
                BlogEnabled = _blogSettings.Enabled,
                CompareProductsEnabled = _catalogSettings.CompareProductsEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                NewsEnabled = _newsSettings.Enabled,
                RecentlyViewedProductsEnabled = _catalogSettings.RecentlyViewedProductsEnabled,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                DisplayTaxShippingInfoFooter = _catalogSettings.DisplayTaxShippingInfoFooter,
                HidePoweredByTvProg = _storeInformationSettings.HidePoweredByTvProgViewer,
                AllowUsersToApplyForVendorAccount = _vendorSettings.AllowUsersToApplyForVendorAccount,
                AllowUsersToCheckGiftCardBalance = _userSettings.AllowUsersToCheckGiftCardBalance && _captchaSettings.Enabled,
                Topics = topicModels,
                DisplaySitemapFooterItem = _displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = _displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayProductSearchFooterItem = _displayDefaultFooterItemSettings.DisplayProductSearchFooterItem,
                DisplayNewsFooterItem = _displayDefaultFooterItemSettings.DisplayNewsFooterItem,
                DisplayBlogFooterItem = _displayDefaultFooterItemSettings.DisplayBlogFooterItem,
                DisplayForumsFooterItem = _displayDefaultFooterItemSettings.DisplayForumsFooterItem,
                DisplayRecentlyViewedProductsFooterItem = _displayDefaultFooterItemSettings.DisplayRecentlyViewedProductsFooterItem,
                DisplayCompareProductsFooterItem = _displayDefaultFooterItemSettings.DisplayCompareProductsFooterItem,
                DisplayNewProductsFooterItem = _displayDefaultFooterItemSettings.DisplayNewProductsFooterItem,
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
        /// <returns>Contact us model</returns>
        public virtual ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentUser.Email;
                model.FullName = _userService.GetUserFullName(_workContext.CurrentUser);
            }

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;

            return model;
        }

        /// <summary>
        /// Prepare the contact vendor model
        /// </summary>
        /// <param name="model">Contact vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact vendor model</returns>
        public virtual ContactVendorModel PrepareContactVendorModel(ContactVendorModel model, Vendor vendor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentUser.Email;
                model.FullName = _userService.GetUserFullName(_workContext.CurrentUser);
            }

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            model.VendorId = vendor.Id;
            model.VendorName = _localizationService.GetLocalized(vendor, x => x.Name);

            return model;
        }

        /// <summary>
        /// Prepare the sitemap model
        /// </summary>
        /// <param name="pageModel">Sitemap page model</param>
        /// <returns>Sitemap model</returns>
        public virtual SitemapModel PrepareSitemapModel(SitemapPageModel pageModel)
        {
            var cacheKey = TvProgModelCacheDefaults.SitemapPageModelKey.FillCacheKey(
                _workContext.WorkingLanguage.Id,
                string.Join(",", _userService.GetUserRoleIds(_workContext.CurrentUser)),
                _storeContext.CurrentStore.Id);

            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                //get URL helper
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                var model = new SitemapModel();

                //prepare common items
                var commonGroupTitle = _localizationService.GetResource("Sitemap.General");

                //home page
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("Homepage"),
                    Url = urlHelper.RouteUrl("Homepage")
                });

                //search
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("Search"),
                    Url = urlHelper.RouteUrl("ProductSearch")
                });

                //news
                if (_newsSettings.Enabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = _localizationService.GetResource("News"),
                        Url = urlHelper.RouteUrl("NewsArchive")
                    });
                }

                //blog
                if (_blogSettings.Enabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = _localizationService.GetResource("Blog"),
                        Url = urlHelper.RouteUrl("Blog")
                    });
                }

                //forums
                if (_forumSettings.ForumsEnabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = _localizationService.GetResource("Forum.Forums"),
                        Url = urlHelper.RouteUrl("Boards")
                    });
                }

                //contact us
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("ContactUs"),
                    Url = urlHelper.RouteUrl("ContactUs")
                });

                //user info
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("Account.MyAccount"),
                    Url = urlHelper.RouteUrl("UserInfo")
                });

                //at the moment topics are in general category too
                if (_sitemapSettings.SitemapIncludeTopics)
                {
                    var topics = _topicService.GetAllTopics(storeId: _storeContext.CurrentStore.Id)
                        .Where(topic => topic.IncludeInSitemap);

                    model.Items.AddRange(topics.Select(topic => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = _localizationService.GetLocalized(topic, x => x.Title),
                        Url = urlHelper.RouteUrl("Topic", new { SeName = _urlRecordService.GetSeName(topic) })
                    }));
                }

                //blog posts
                if (_sitemapSettings.SitemapIncludeBlogPosts && _blogSettings.Enabled)
                {
                    var blogPostsGroupTitle = _localizationService.GetResource("Sitemap.BlogPosts");
                    var blogPosts = _blogService.GetAllBlogPosts(storeId: _storeContext.CurrentStore.Id)
                        .Where(p => p.IncludeInSitemap);

                    model.Items.AddRange(blogPosts.Select(post => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = blogPostsGroupTitle,
                        Name = post.Title,
                        Url = urlHelper.RouteUrl("BlogPost", new { SeName = _urlRecordService.GetSeName(post) })
                    }));
                }

                //news
                if (_sitemapSettings.SitemapIncludeNews && _newsSettings.Enabled)
                {
                    var newsGroupTitle = _localizationService.GetResource("Sitemap.News");
                    var news = _newsService.GetAllNews(storeId: _storeContext.CurrentStore.Id);
                    model.Items.AddRange(news.Select(newsItem => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = newsGroupTitle,
                        Name = newsItem.Title,
                        Url = urlHelper.RouteUrl("NewsItem", new { SeName = _urlRecordService.GetSeName(newsItem) })
                    }));
                }

                //categories
                if (_sitemapSettings.SitemapIncludeCategories)
                {
                    var categoriesGroupTitle = _localizationService.GetResource("Sitemap.Categories");
                    var categories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                    model.Items.AddRange(categories.Select(category => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = categoriesGroupTitle,
                        Name = _localizationService.GetLocalized(category, x => x.Name),
                        Url = urlHelper.RouteUrl("Category", new { SeName = _urlRecordService.GetSeName(category) })
                    }));
                }

                //manufacturers
                if (_sitemapSettings.SitemapIncludeManufacturers)
                {
                    var manufacturersGroupTitle = _localizationService.GetResource("Sitemap.Manufacturers");
                    var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);
                    model.Items.AddRange(manufacturers.Select(manufacturer => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = manufacturersGroupTitle,
                        Name = _localizationService.GetLocalized(manufacturer, x => x.Name),
                        Url = urlHelper.RouteUrl("Manufacturer", new { SeName = _urlRecordService.GetSeName(manufacturer) })
                    }));
                }

                //products
                if (_sitemapSettings.SitemapIncludeProducts)
                {
                    var productsGroupTitle = _localizationService.GetResource("Sitemap.Products");
                    var products = _productService.SearchProducts(storeId: _storeContext.CurrentStore.Id, visibleIndividuallyOnly: true);
                    model.Items.AddRange(products.Select(product => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = productsGroupTitle,
                        Name = _localizationService.GetLocalized(product, x => x.Name),
                        Url = urlHelper.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                    }));
                }

                //product tags
                if (_sitemapSettings.SitemapIncludeProductTags)
                {
                    var productTagsGroupTitle = _localizationService.GetResource("Sitemap.ProductTags");
                    var productTags = _productTagService.GetAllProductTags();
                    model.Items.AddRange(productTags.Select(productTag => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = productTagsGroupTitle,
                        Name = _localizationService.GetLocalized(productTag, x => x.Name),
                        Url = urlHelper.RouteUrl("ProductsByTag", new { SeName = _urlRecordService.GetSeName(productTag) })
                    }));
                }

                return model;
            });

            //prepare model with pagination
            pageModel.PageSize = Math.Max(pageModel.PageSize, _sitemapSettings.SitemapPageSize);
            pageModel.PageNumber = Math.Max(pageModel.PageNumber, 1);

            var pagedItems = new PagedList<SitemapModel.SitemapItemModel>(cachedModel.Items, pageModel.PageNumber - 1, pageModel.PageSize);
            var sitemapModel = new SitemapModel { Items = pagedItems };
            sitemapModel.PageModel.LoadPagedList(pagedItems);

            return sitemapModel;
        }

        /// <summary>
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        public virtual string PrepareSitemapXml(int? id)
        {
            var cacheKey = TvProgModelCacheDefaults.SitemapSeoModelKey.FillCacheKey(id,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _userService.GetUserRoleIds(_workContext.CurrentUser)),
                _storeContext.CurrentStore.Id);

            var siteMap = _cacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(id));

            return siteMap;
        }

        /// <summary>
        /// Prepare the store theme selector model
        /// </summary>
        /// <returns>Store theme selector model</returns>
        public virtual StoreThemeSelectorModel PrepareStoreThemeSelectorModel()
        {
            var model = new StoreThemeSelectorModel();

            var currentTheme = _themeProvider.GetThemeBySystemName(_themeContext.WorkingThemeName);
            model.CurrentStoreTheme = new StoreThemeModel
            {
                Name = currentTheme?.SystemName,
                Title = currentTheme?.FriendlyName
            };

            model.AvailableStoreThemes = _themeProvider.GetThemes().Select(x => new StoreThemeModel
            {
                Name = x.SystemName,
                Title = x.FriendlyName
            }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>Favicon model</returns>
        public virtual FaviconAndAppIconsModel PrepareFaviconAndAppIconsModel()
        {
            var model = new FaviconAndAppIconsModel
            {
                HeadCode = _commonSettings.FaviconAndAppIconsHeadCode
            };

            return model;
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        public virtual string PrepareRobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.custom.txt");
            if (_fileProvider.FileExists(robotsFilePath))
            {
                //the robots.txt file exists
                var robotsFileContent = _fileProvider.ReadAllText(robotsFilePath, Encoding.UTF8);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/admin",
                    "/bin/",
                    "/files/",
                    "/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/setproductreviewhelpfulness",
                };
                var localizableDisallowPaths = new List<string>
                {
                    "/addproducttocart/catalog/",
                    "/addproducttocart/details/",
                    "/backinstocksubscriptions/manage",
                    "/boards/forumsubscriptions",
                    "/boards/forumwatch",
                    "/boards/postedit",
                    "/boards/postdelete",
                    "/boards/postcreate",
                    "/boards/topicedit",
                    "/boards/topicdelete",
                    "/boards/topiccreate",
                    "/boards/topicmove",
                    "/boards/topicwatch",
                    "/cart$",
                    "/changecurrency",
                    "/changelanguage",
                    "/changetaxtype",
                    "/changeprovider",
                    "/checkout",
                    "/checkout/billingaddress",
                    "/checkout/completed",
                    "/checkout/confirm",
                    "/checkout/shippingaddress",
                    "/checkout/shippingmethod",
                    "/checkout/paymentinfo",
                    "/checkout/paymentmethod",
                    "/clearcomparelist",
                    "/compareproducts",
                    "/compareproducts/add/*",
                    "/user/avatar",
                    "/user/activation",
                    "/user/addresses",
                    "/user/changepassword",
                    "/user/checkusernameavailability",
                    "/user/downloadableproducts",
                    "/user/info",
                    "/deletepm",
                    "/emailwishlist",
                    "/eucookielawaccept",
                    "/inboxupdate",
                    "/newsletter/subscriptionactivation",
                    "/onepagecheckout",
                    "/order/history",
                    "/orderdetails",
                    "/passwordrecovery/confirm",
                    "/poll/vote",
                    "/privatemessages",
                    "/returnrequest",
                    "/returnrequest/history",
                    "/rewardpoints/history",
                    "/search?",
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/storeclosed",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfilecheckoutattribute",
                    "/uploadfileproductattribute",
                    "/uploadfilereturnrequest",
                    "/wishlist",
                };

                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);
                //sitemaps
                if (_sitemapXmlSettings.SitemapXmlEnabled)
                {
                    if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    {
                        //URLs are localizable. Append SEO code
                        foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                        {
                            sb.AppendFormat("Sitemap: {0}{1}/sitemap.xml", _webHelper.GetStoreLocation(), language.UniqueSeoCode);
                            sb.Append(newLine);
                        }
                    }
                    else
                    {
                        //localizable paths (without SEO code)
                        sb.AppendFormat("Sitemap: {0}sitemap.xml", _webHelper.GetStoreLocation());
                        sb.Append(newLine);
                    }
                }
                //host
                sb.AppendFormat("Host: {0}", _webHelper.GetStoreLocation());
                sb.Append(newLine);

                //usual paths
                foreach (var path in disallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                //localizable paths (without SEO code)
                foreach (var path in localizableDisallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                    {
                        foreach (var path in localizableDisallowPaths)
                        {
                            sb.AppendFormat("Disallow: /{0}{1}", language.UniqueSeoCode, path);
                            sb.Append(newLine);
                        }
                    }
                }

                //load and add robots.txt additions to the end of file.
                var robotsAdditionsFile = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.additions.txt");
                if (_fileProvider.FileExists(robotsAdditionsFile))
                {
                    var robotsFileContent = _fileProvider.ReadAllText(robotsAdditionsFile, Encoding.UTF8);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}