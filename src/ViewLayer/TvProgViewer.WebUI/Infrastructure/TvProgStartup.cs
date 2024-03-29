﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Helpers;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.WebUI.Infrastructure.Installation;

namespace TvProgViewer.WebUI.Infrastructure
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
            //installation localization service
            services.AddScoped<IInstallationLocalizationService, InstallationLocalizationService>();

            //common factories
            services.AddScoped<IAclSupportedModelFactory, AclSupportedModelFactory>();
            services.AddScoped<IDiscountSupportedModelFactory, DiscountSupportedModelFactory>();
            services.AddScoped<ILocalizedModelFactory, LocalizedModelFactory>();
            services.AddScoped<IStoreMappingSupportedModelFactory, StoreMappingSupportedModelFactory>();

            //admin factories
            services.AddScoped<IBaseAdminModelFactory, BaseAdminModelFactory>();
            services.AddScoped<IActivityLogModelFactory, ActivityLogModelFactory>();
            services.AddScoped<IAddressModelFactory, AddressModelFactory>();
            services.AddScoped<IAddressAttributeModelFactory, AddressAttributeModelFactory>();
            services.AddScoped<IAffiliateModelFactory, AffiliateModelFactory>();
            services.AddScoped<IBlogModelFactory, BlogModelFactory>();
            services.AddScoped<ICampaignModelFactory, CampaignModelFactory>();
            services.AddScoped<ICategoryModelFactory, CategoryModelFactory>();
            services.AddScoped<ICheckoutAttributeModelFactory, CheckoutAttributeModelFactory>();
            services.AddScoped<ICommonModelFactory, CommonModelFactory>();
            services.AddScoped<ICountryModelFactory, CountryModelFactory>();
            services.AddScoped<ICurrencyModelFactory, CurrencyModelFactory>();
            services.AddScoped<IUserAttributeModelFactory, UserAttributeModelFactory>();
            services.AddScoped<IUserModelFactory, UserModelFactory>();
            services.AddScoped<IUserRoleModelFactory, UserRoleModelFactory>();
            services.AddScoped<IDiscountModelFactory, DiscountModelFactory>();
            services.AddScoped<IEmailAccountModelFactory, EmailAccountModelFactory>();
            services.AddScoped<IExternalAuthenticationMethodModelFactory, ExternalAuthenticationMethodModelFactory>();
            services.AddScoped<IForumModelFactory, ForumModelFactory>();
            services.AddScoped<IGiftCardModelFactory, GiftCardModelFactory>();
            services.AddScoped<IHomeModelFactory, HomeModelFactory>();
            services.AddScoped<ILanguageModelFactory, LanguageModelFactory>();
            services.AddScoped<ILogModelFactory, LogModelFactory>();
            services.AddScoped<IManufacturerModelFactory, ManufacturerModelFactory>();
            services.AddScoped<IMeasureModelFactory, MeasureModelFactory>();
            services.AddScoped<IMessageTemplateModelFactory, MessageTemplateModelFactory>();
            services.AddScoped<IMultiFactorAuthenticationMethodModelFactory, MultiFactorAuthenticationMethodModelFactory>();
            services.AddScoped<INewsletterSubscriptionModelFactory, NewsletterSubscriptionModelFactory>();
            services.AddScoped<INewsModelFactory, NewsModelFactory>();
            services.AddScoped<IOrderModelFactory, OrderModelFactory>();
            services.AddScoped<IPaymentModelFactory, PaymentModelFactory>();
            services.AddScoped<IPluginModelFactory, PluginModelFactory>();
            services.AddScoped<IPollModelFactory, PollModelFactory>();
            services.AddScoped<ITvChannelModelFactory, TvChannelModelFactory>();
            services.AddScoped<ITvChannelAttributeModelFactory, TvChannelAttributeModelFactory>();
            services.AddScoped<ITvChannelReviewModelFactory, TvChannelReviewModelFactory>();
            services.AddScoped<IReportModelFactory, ReportModelFactory>();
            services.AddScoped<IQueuedEmailModelFactory, QueuedEmailModelFactory>();
            services.AddScoped<IRecurringPaymentModelFactory, RecurringPaymentModelFactory>();
            services.AddScoped<IReturnRequestModelFactory, ReturnRequestModelFactory>();
            services.AddScoped<IReviewTypeModelFactory, ReviewTypeModelFactory>();
            services.AddScoped<IScheduleTaskModelFactory, ScheduleTaskModelFactory>();
            services.AddScoped<ISecurityModelFactory, SecurityModelFactory>();
            services.AddScoped<ISettingModelFactory, SettingModelFactory>();
            services.AddScoped<IShippingModelFactory, ShippingModelFactory>();
            services.AddScoped<IShoppingCartModelFactory, ShoppingCartModelFactory>();
            services.AddScoped<ISpecificationAttributeModelFactory, SpecificationAttributeModelFactory>();
            services.AddScoped<IStoreModelFactory, StoreModelFactory>();
            services.AddScoped<ITaxModelFactory, TaxModelFactory>();
            services.AddScoped<ITemplateModelFactory, TemplateModelFactory>();
            services.AddScoped<ITopicModelFactory, TopicModelFactory>();
            services.AddScoped<IVendorAttributeModelFactory, VendorAttributeModelFactory>();
            services.AddScoped<IVendorModelFactory, VendorModelFactory>();
            services.AddScoped<IWidgetModelFactory, WidgetModelFactory>();

            //factories
            services.AddScoped<Factories.IAddressModelFactory, Factories.AddressModelFactory>();
            services.AddScoped<Factories.IBlogModelFactory, Factories.BlogModelFactory>();
            services.AddScoped<Factories.ICatalogModelFactory, Factories.CatalogModelFactory>();
            services.AddScoped<Factories.ICheckoutModelFactory, Factories.CheckoutModelFactory>();
            services.AddScoped<Factories.ICommonModelFactory, Factories.CommonModelFactory>();
            services.AddScoped<Factories.ICountryModelFactory, Factories.CountryModelFactory>();
            services.AddScoped<Factories.IUserModelFactory, Factories.UserModelFactory>();
            services.AddScoped<Factories.IForumModelFactory, Factories.ForumModelFactory>();
            services.AddScoped<Factories.IExternalAuthenticationModelFactory, Factories.ExternalAuthenticationModelFactory>();
            services.AddScoped<Factories.INewsModelFactory, Factories.NewsModelFactory>();
            services.AddScoped<Factories.INewsletterModelFactory, Factories.NewsletterModelFactory>();
            services.AddScoped<Factories.IOrderModelFactory, Factories.OrderModelFactory>();
            services.AddScoped<Factories.IPollModelFactory, Factories.PollModelFactory>();
            services.AddScoped<Factories.IPrivateMessagesModelFactory, Factories.PrivateMessagesModelFactory>();
            services.AddScoped<Factories.ITvChannelModelFactory, Factories.TvChannelModelFactory>();
            services.AddScoped<Factories.IProfileModelFactory, Factories.ProfileModelFactory>();
            services.AddScoped<Factories.IReturnRequestModelFactory, Factories.ReturnRequestModelFactory>();
            services.AddScoped<Factories.IShoppingCartModelFactory, Factories.ShoppingCartModelFactory>();
            services.AddScoped<Factories.ISitemapModelFactory, Factories.SitemapModelFactory>();
            services.AddScoped<Factories.ITopicModelFactory, Factories.TopicModelFactory>();
            services.AddScoped<Factories.IVendorModelFactory, Factories.VendorModelFactory>();
            services.AddScoped<Factories.IWidgetModelFactory, Factories.WidgetModelFactory>();

            //helpers classes
            services.AddScoped<ITinyMceHelper, TinyMceHelper>();
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
        public int Order => 2002;
    }
}
