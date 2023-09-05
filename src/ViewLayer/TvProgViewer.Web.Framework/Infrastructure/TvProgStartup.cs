using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Events;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Services.Affiliates;
using TvProgViewer.Services.Authentication;
using TvProgViewer.Services.Authentication.External;
using TvProgViewer.Services.Authentication.MultiFactor;
using TvProgViewer.Services.Blogs;
using TvProgViewer.Services.Caching;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Cms;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Events;
using TvProgViewer.Services.ExportImport;
using TvProgViewer.Services.Forums;
using TvProgViewer.Services.Gdpr;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Installation;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Media.RoxyFileman;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.News;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Services.Plugins.Marketplace;
using TvProgViewer.Services.Polls;
using TvProgViewer.Services.ScheduleTasks;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Shipping.Pickup;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Tax;
using TvProgViewer.Services.Themes;
using TvProgViewer.Services.Topics;
using TvProgViewer.Services.Vendors;
using TvProgViewer.Web.Framework.Menu;
using TvProgViewer.Web.Framework.Mvc.Routing;
using TvProgViewer.Web.Framework.Themes;
using TvProgViewer.Web.Framework.UI;
using TvProgViewer.Services.TvProgMain;
using System.Linq;

namespace TvProgViewer.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents the registering services on application startup
    /// </summary>
    public partial class TvProgStartup : ITvProgStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //file provider
            services.AddScoped<ITvProgFileProvider, TvProgFileProvider>();

            //web helper
            services.AddScoped<IWebHelper, WebHelper>();

            //user agent helper
            services.AddScoped<IUserAgentHelper, UserAgentHelper>();

            //plugins
            services.AddScoped<IPluginService, PluginService>();
            services.AddScoped<OfficialFeedManager>();

            //static cache manager
            var appSettings = Singleton<AppSettings>.Instance;
            var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();
            if (distributedCacheConfig.Enabled)
            {
                switch (distributedCacheConfig.DistributedCacheType)
                {
                    case DistributedCacheType.Memory:
                        services.AddScoped<ILocker, MemoryDistributedCacheManager>();
                        services.AddScoped<IStaticCacheManager, MemoryDistributedCacheManager>();
                        break;
                    case DistributedCacheType.SqlServer:
                        services.AddScoped<ILocker, MsSqlServerCacheManager>();
                        services.AddScoped<IStaticCacheManager, MsSqlServerCacheManager>();
                        break;
                    case DistributedCacheType.Redis:
                        services.AddScoped<ILocker, RedisCacheManager>();
                        services.AddScoped<IStaticCacheManager, RedisCacheManager>();
                        break;
                }
            }
            else
            {
                services.AddSingleton<ILocker, MemoryCacheManager>();
                services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            }

            //work context
            services.AddScoped<IWorkContext, WebWorkContext>();

            //store context
            services.AddScoped<IStoreContext, WebStoreContext>();

            //services
            services.AddScoped<IBackInStockSubscriptionService, BackInStockSubscriptionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICompareProductsService, CompareProductsService>();
            services.AddScoped<IRecentlyViewedProductsService, RecentlyViewedProductsService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IPriceFormatter, PriceFormatter>();
            services.AddScoped<IProductAttributeFormatter, ProductAttributeFormatter>();
            services.AddScoped<IProductAttributeParser, ProductAttributeParser>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICopyProductService, CopyProductService>();
            services.AddScoped<ISpecificationAttributeService, SpecificationAttributeService>();
            services.AddScoped<IProductTemplateService, ProductTemplateService>();
            services.AddScoped<ICategoryTemplateService, CategoryTemplateService>();
            services.AddScoped<IManufacturerTemplateService, ManufacturerTemplateService>();
            services.AddScoped<ITopicTemplateService, TopicTemplateService>();
            services.AddScoped<IProductTagService, ProductTagService>();
            services.AddScoped<IAddressAttributeFormatter, AddressAttributeFormatter>();
            services.AddScoped<IAddressAttributeParser, AddressAttributeParser>();
            services.AddScoped<IAddressAttributeService, AddressAttributeService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAffiliateService, AffiliateService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<IVendorAttributeFormatter, VendorAttributeFormatter>();
            services.AddScoped<IVendorAttributeParser, VendorAttributeParser>();
            services.AddScoped<IVendorAttributeService, VendorAttributeService>();
            services.AddScoped<ISearchTermService, SearchTermService>();
            services.AddScoped<IGenericAttributeService, GenericAttributeService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IUserAttributeFormatter, UserAttributeFormatter>();
            services.AddScoped<IUserAttributeParser, UserAttributeParser>();
            services.AddScoped<IUserAttributeService, UserAttributeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<IUserReportService, UserReportService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAclService, AclService>();
            services.AddScoped<IPriceCalculationService, PriceCalculationService>();
            services.AddScoped<IGeoLookupService, GeoLookupService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IMeasureService, MeasureService>();
            services.AddScoped<IStateProvinceService, StateProvinceService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStoreMappingService, StoreMappingService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IDownloadService, DownloadService>();
            services.AddScoped<IMessageTemplateService, MessageTemplateService>();
            services.AddScoped<IQueuedEmailService, QueuedEmailService>();
            services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
            services.AddScoped<IMessageTokenProvider, MessageTokenProvider>();
            services.AddScoped<ITokenizer, Tokenizer>();
            services.AddScoped<ISmtpBuilder, SmtpBuilder>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ICheckoutAttributeFormatter, CheckoutAttributeFormatter>();
            services.AddScoped<ICheckoutAttributeParser, CheckoutAttributeParser>();
            services.AddScoped<ICheckoutAttributeService, CheckoutAttributeService>();
            services.AddScoped<IGiftCardService, GiftCardService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderReportService, OrderReportService>();
            services.AddScoped<IOrderProcessingService, OrderProcessingService>();
            services.AddScoped<IOrderTotalCalculationService, OrderTotalCalculationService>();
            services.AddScoped<IReturnRequestService, ReturnRequestService>();
            services.AddScoped<IRewardPointService, RewardPointService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<ICustomNumberFormatter, CustomNumberFormatter>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAuthenticationService, CookieAuthenticationService>();
            services.AddScoped<IUrlRecordService, UrlRecordService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IShippingService, ShippingService>();
            services.AddScoped<IDateRangeService, DateRangeService>();
            services.AddScoped<ITaxCategoryService, TaxCategoryService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<ILogger, DefaultLogger>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            services.AddScoped<IForumService, ForumService>();
            services.AddScoped<IGdprService, GdprService>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IDateTimeHelper, DateTimeHelper>();
            services.AddScoped<ITvProgHtmlHelper, TvProgHtmlHelper>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            services.AddScoped<IExportManager, ExportManager>();
            services.AddScoped<IImportManager, ImportManager>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IThemeProvider, ThemeProvider>();
            services.AddScoped<IThemeContext, ThemeContext>();
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddSingleton<IRoutePublisher, RoutePublisher>();
            services.AddScoped<IReviewTypeService, ReviewTypeService>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IBBCodeHelper, BBCodeHelper>();
            services.AddScoped<IHtmlFormatter, HtmlFormatter>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<ITvProgUrlHelper, TvProgUrlHelper>();
            services.AddScoped<IProgrammeService, ProgrammeService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IUpdaterService, UpdaterService>();

            //plugin managers
            services.AddScoped(typeof(IPluginManager<>), typeof(PluginManager<>));
            services.AddScoped<IAuthenticationPluginManager, AuthenticationPluginManager>();
            services.AddScoped<IMultiFactorAuthenticationPluginManager, MultiFactorAuthenticationPluginManager>();
            services.AddScoped<IWidgetPluginManager, WidgetPluginManager>();
            services.AddScoped<IExchangeRatePluginManager, ExchangeRatePluginManager>();
            services.AddScoped<IDiscountPluginManager, DiscountPluginManager>();
            services.AddScoped<IPaymentPluginManager, PaymentPluginManager>();
            services.AddScoped<IPickupPluginManager, PickupPluginManager>();
            services.AddScoped<IShippingPluginManager, ShippingPluginManager>();
            services.AddScoped<ITaxPluginManager, TaxPluginManager>();
            services.AddScoped<ISearchPluginManager, SearchPluginManager>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //register all settings
            var typeFinder = Singleton<ITypeFinder>.Instance;

            var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
            foreach (var setting in settings)
            {
                services.AddScoped(setting, serviceProvider =>
                {
                    var storeId = DataSettingsManager.IsDatabaseInstalled()
                        ? serviceProvider.GetRequiredService<IStoreContext>().GetCurrentStore()?.Id ?? 0
                        : 0;

                    return serviceProvider.GetRequiredService<ISettingService>().LoadSettingAsync(setting, storeId).Result;
                });
            }

            //picture service
            if (appSettings.Get<AzureBlobConfig>().Enabled)
                services.AddScoped<IPictureService, AzurePictureService>();
            else
                services.AddScoped<IPictureService, PictureService>();

            //roxy file manager
            services.AddScoped<IRoxyFilemanService, RoxyFilemanService>();
            services.AddScoped<IRoxyFilemanFileProvider, RoxyFilemanFileProvider>();

            //installation service
            services.AddScoped<IInstallationService, InstallationService>();

            //slug route transformer
            if (DataSettingsManager.IsDatabaseInstalled())
                services.AddScoped<SlugRouteTransformer>();

            //schedule tasks
            services.AddSingleton<ITaskScheduler, TaskScheduler>();
            services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
                foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(IConsumer<>)))
                    services.AddScoped(findInterface, consumer);

            //XML sitemap
            services.AddScoped<IXmlSiteMap, XmlSiteMap>();

            //register the Lazy resolver for .Net IoC
            var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;
            if (!useAutofac)
                services.AddScoped(typeof(Lazy<>), typeof(LazyInstance<>));
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 2000;
    }
}