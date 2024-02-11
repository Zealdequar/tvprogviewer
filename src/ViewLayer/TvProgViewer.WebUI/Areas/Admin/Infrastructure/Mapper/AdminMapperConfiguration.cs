using AutoMapper;
using AutoMapper.Internal;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Domain.Affiliates;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Configuration;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Logging;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Polls;
using TvProgViewer.Core.Domain.ScheduleTasks;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Infrastructure.Mapper;
using TvProgViewer.Data.Configuration;
using TvProgViewer.Services.Authentication.External;
using TvProgViewer.Services.Authentication.MultiFactor;
using TvProgViewer.Services.Cms;
using TvProgViewer.Services.Payments;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Shipping.Pickup;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Areas.Admin.Models.Affiliates;
using TvProgViewer.WebUI.Areas.Admin.Models.Blogs;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Cms;
using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.WebUI.Areas.Admin.Models.Users;
using TvProgViewer.WebUI.Areas.Admin.Models.Directory;
using TvProgViewer.WebUI.Areas.Admin.Models.Discounts;
using TvProgViewer.WebUI.Areas.Admin.Models.ExternalAuthentication;
using TvProgViewer.WebUI.Areas.Admin.Models.Forums;
using TvProgViewer.WebUI.Areas.Admin.Models.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Logging;
using TvProgViewer.WebUI.Areas.Admin.Models.Messages;
using TvProgViewer.WebUI.Areas.Admin.Models.MultiFactorAuthentication;
using TvProgViewer.WebUI.Areas.Admin.Models.News;
using TvProgViewer.WebUI.Areas.Admin.Models.Orders;
using TvProgViewer.WebUI.Areas.Admin.Models.Payments;
using TvProgViewer.WebUI.Areas.Admin.Models.Plugins;
using TvProgViewer.WebUI.Areas.Admin.Models.Polls;
using TvProgViewer.WebUI.Areas.Admin.Models.Settings;
using TvProgViewer.WebUI.Areas.Admin.Models.Shipping;
using TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart;
using TvProgViewer.WebUI.Areas.Admin.Models.Stores;
using TvProgViewer.WebUI.Areas.Admin.Models.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Tax;
using TvProgViewer.WebUI.Areas.Admin.Models.Templates;
using TvProgViewer.WebUI.Areas.Admin.Models.Topics;
using TvProgViewer.WebUI.Areas.Admin.Models.Vendors;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.WebOptimizer;

namespace TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public partial class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public AdminMapperConfiguration()
        {
            //create specific maps
            CreateConfigMaps();
            CreateAffiliatesMaps();
            CreateAuthenticationMaps();
            CreateMultiFactorAuthenticationMaps();
            CreateBlogsMaps();
            CreateCatalogMaps();
            CreateCmsMaps();
            CreateCommonMaps();
            CreateUsersMaps();
            CreateDirectoryMaps();
            CreateDiscountsMaps();
            CreateForumsMaps();
            CreateGdprMaps();
            CreateLocalizationMaps();
            CreateLoggingMaps();
            CreateMediaMaps();
            CreateMessagesMaps();
            CreateNewsMaps();
            CreateOrdersMaps();
            CreatePaymentsMaps();
            CreatePluginsMaps();
            CreatePollsMaps();
            CreateSecurityMaps();
            CreateSeoMaps();
            CreateShippingMaps();
            CreateStoresMaps();
            CreateTasksMaps();
            CreateTaxMaps();
            CreateTopicsMaps();
            CreateVendorsMaps();
            CreateWarehouseMaps();

            //add some generic mapping rules
            this.Internal().ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseTvProgModel
                if (typeof(BaseTvProgModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    //map.ForMember(nameof(BaseTvProgModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseTvProgModel.CustomProperties), options => options.Ignore());
                }

                //exclude ActiveStoreScopeConfiguration from mapping ISettingsModel
                if (typeof(ISettingsModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ISettingsModel.ActiveStoreScopeConfiguration), options => options.Ignore());

                //exclude some properties from mapping configuration and models
                if (typeof(IConfig).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IConfig.Name), options => options.Ignore());

                //exclude Locales from mapping ILocalizedModel
                if (typeof(ILocalizedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ILocalizedModel<ILocalizedModel>.Locales), options => options.Ignore());

                //exclude some properties from mapping store mapping supported entities and models
                if (typeof(IStoreMappingSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IStoreMappingSupported.LimitedToStores), options => options.Ignore());
                if (typeof(IStoreMappingSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IStoreMappingSupportedModel.AvailableStores), options => options.Ignore());
                    map.ForMember(nameof(IStoreMappingSupportedModel.SelectedStoreIds), options => options.Ignore());
                }

                //exclude some properties from mapping ACL supported entities and models
                if (typeof(IAclSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IAclSupported.SubjectToAcl), options => options.Ignore());
                if (typeof(IAclSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IAclSupportedModel.AvailableUserRoles), options => options.Ignore());
                    map.ForMember(nameof(IAclSupportedModel.SelectedUserRoleIds), options => options.Ignore());
                }

                //exclude some properties from mapping discount supported entities and models
                if (typeof(IDiscountSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IDiscountSupportedModel.AvailableDiscounts), options => options.Ignore());
                    map.ForMember(nameof(IDiscountSupportedModel.SelectedDiscountIds), options => options.Ignore());
                }

                if (typeof(IPluginModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    //exclude some properties from mapping plugin models
                    map.ForMember(nameof(IPluginModel.ConfigurationUrl), options => options.Ignore());
                    map.ForMember(nameof(IPluginModel.IsActive), options => options.Ignore());
                    map.ForMember(nameof(IPluginModel.LogoUrl), options => options.Ignore());

                    //define specific rules for mapping plugin models
                    if (typeof(IPlugin).IsAssignableFrom(mapConfiguration.SourceType))
                    {
                        map.ForMember(nameof(IPluginModel.DisplayOrder), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.DisplayOrder));
                        map.ForMember(nameof(IPluginModel.FriendlyName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.FriendlyName));
                        map.ForMember(nameof(IPluginModel.SystemName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.SystemName));
                    }
                }
            });
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create configuration maps 
        /// </summary>
        protected virtual void CreateConfigMaps()
        {
            CreateMap<CacheConfig, CacheConfigModel>();
            CreateMap<CacheConfigModel, CacheConfig>();

            CreateMap<HostingConfig, HostingConfigModel>();
            CreateMap<HostingConfigModel, HostingConfig>();

            CreateMap<DistributedCacheConfig, DistributedCacheConfigModel>()
                .ForMember(model => model.DistributedCacheTypeValues, options => options.Ignore());
            CreateMap<DistributedCacheConfigModel, DistributedCacheConfig>();

            CreateMap<AzureBlobConfig, AzureBlobConfigModel>();
            CreateMap<AzureBlobConfigModel, AzureBlobConfig>()
                .ForMember(entity => entity.Enabled, options => options.Ignore())
                .ForMember(entity => entity.DataProtectionKeysEncryptWithVault, options => options.Ignore());

            CreateMap<InstallationConfig, InstallationConfigModel>();
            CreateMap<InstallationConfigModel, InstallationConfig>();

            CreateMap<PluginConfig, PluginConfigModel>();
            CreateMap<PluginConfigModel, PluginConfig>();

            CreateMap<CommonConfig, CommonConfigModel>();
            CreateMap<CommonConfigModel, CommonConfig>();

            CreateMap<DataConfig, DataConfigModel>()
                .ForMember(model => model.DataProviderTypeValues, options => options.Ignore());
            CreateMap<DataConfigModel, DataConfig>();

            CreateMap<WebOptimizerConfig, WebOptimizerConfigModel>();
            CreateMap<WebOptimizerConfigModel, WebOptimizerConfig>()
                .ForMember(entity => entity.CdnUrl, options => options.Ignore())
                .ForMember(entity => entity.AllowEmptyBundle, options => options.Ignore())
                .ForMember(entity => entity.HttpsCompression, options => options.Ignore())
                .ForMember(entity => entity.EnableTagHelperBundling, options => options.Ignore())
                .ForMember(entity => entity.EnableCaching, options => options.Ignore())
                .ForMember(entity => entity.EnableMemoryCache, options => options.Ignore());
        }

        /// <summary>
        /// Create affiliates maps 
        /// </summary>
        protected virtual void CreateAffiliatesMaps()
        {
            CreateMap<Affiliate, AffiliateModel>()
                .ForMember(model => model.Address, options => options.Ignore())
                .ForMember(model => model.AffiliatedUserSearchModel, options => options.Ignore())
                .ForMember(model => model.AffiliatedOrderSearchModel, options => options.Ignore())
                .ForMember(model => model.Url, options => options.Ignore());
            CreateMap<AffiliateModel, Affiliate>()
                .ForMember(entity => entity.Deleted, options => options.Ignore());

            CreateMap<Order, AffiliatedOrderModel>()
                .ForMember(model => model.OrderStatus, options => options.Ignore())
                .ForMember(model => model.PaymentStatus, options => options.Ignore())
                .ForMember(model => model.ShippingStatus, options => options.Ignore())
                .ForMember(model => model.OrderTotal, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());

            CreateMap<User, AffiliatedUserModel>()
                .ForMember(model => model.Name, options => options.Ignore());

        }

        /// <summary>
        /// Create authentication maps 
        /// </summary>
        protected virtual void CreateAuthenticationMaps()
        {
            CreateMap<IExternalAuthenticationMethod, ExternalAuthenticationMethodModel>();
        }

        /// <summary>
        /// Create multi-factor authentication maps 
        /// </summary>
        protected virtual void CreateMultiFactorAuthenticationMaps()
        {
            CreateMap<IMultiFactorAuthenticationMethod, MultiFactorAuthenticationMethodModel>();
        }

        /// <summary>
        /// Create blogs maps 
        /// </summary>new
        protected virtual void CreateBlogsMaps()
        {
            CreateMap<BlogComment, BlogCommentModel>()
                .ForMember(model => model.BlogPostTitle, options => options.Ignore())
                .ForMember(model => model.Comment, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.UserInfo, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());

            CreateMap<BlogCommentModel, BlogComment>()
                .ForMember(entity => entity.CommentText, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.BlogPostId, options => options.Ignore())
                .ForMember(entity => entity.UserId, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore());

            CreateMap<BlogPost, BlogPostModel>()
                .ForMember(model => model.ApprovedComments, options => options.Ignore())
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LanguageName, options => options.Ignore())
                .ForMember(model => model.NotApprovedComments, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.InitialBlogTags, options => options.Ignore());
            CreateMap<BlogPostModel, BlogPost>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

            CreateMap<BlogSettings, BlogSettingsModel>()
                .ForMember(model => model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BlogCommentsMustBeApproved_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyAboutNewBlogComments_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfTags_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PostsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowHeaderRssUrl_OverrideForStore, options => options.Ignore());
            CreateMap<BlogSettingsModel, BlogSettings>();
        }

        /// <summary>
        /// Create catalog maps 
        /// </summary>
        protected virtual void CreateCatalogMaps()
        {
            CreateMap<CatalogSettings, CatalogSettingsModel>()
                .ForMember(model => model.AllowAnonymousUsersToEmailAFriend_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowAnonymousUsersToReviewTvChannel_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowTvChannelSorting_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowTvChannelViewModeChanging_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowViewUnpublishedTvChannelPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AvailableViewModes, options => options.Ignore())
                .ForMember(model => model.CategoryBreadcrumbEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CompareTvChannelsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultViewMode_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayDiscontinuedMessageForUnpublishedTvChannels_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoFooter_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoTvChannelBoxes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoTvChannelDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoShoppingCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxShippingInfoWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EmailAFriendEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportAllowDownloadImages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportCategoriesUsingCategoryName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportTvChannelAttributes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportTvChannelCategoryBreadcrumb_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportTvChannelSpecificationAttributes_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportRelatedEntitiesByName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportTvChannelUseLimitedToStores_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportImportSplitTvChannelsFile_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IncludeFullDescriptionInCompareTvChannels_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IncludeShortDescriptionInCompareTvChannels_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ManufacturersBlockItemsToDisplay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewTvChannelsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewTvChannelsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewTvChannelsAllowUsersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewTvChannelsPageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyUserAboutTvChannelReviewReply_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyStoreOwnerAboutNewTvChannelReviews_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfBestsellersOnHomepage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfTvChannelTags_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PageShareCode_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelReviewPossibleOnlyAfterPurchasing_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelReviewsMustBeApproved_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OneReviewPerTvChannelFromUser_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelReviewsPageSizeOnAccountPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelReviewsSortByCreatedDateAscending_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelsAlsoPurchasedEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelsAlsoPurchasedNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelsByTagAllowUsersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelsByTagPageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelsByTagPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelSearchAutoCompleteEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelSearchEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelSearchAutoCompleteNumberOfTvChannels_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelSearchTermMinimumLength_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RecentlyViewedTvChannelsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RecentlyViewedTvChannelsNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RemoveRequiredTvChannels_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPageAllowUsersToSelectPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPagePageSizeOptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchPageTvChannelsPerPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowBestsellersOnHomepage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowCategoryTvChannelNumberIncludingSubcategories_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowCategoryTvChannelNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowFreeShippingNotification_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowShortDescriptionOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowGtin_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowManufacturerPartNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowTvChannelImagesInSearchAutoComplete_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowTvChannelReviewsOnAccountPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowTvChannelReviewsPerStore_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowTvChannelsFromSubcategories_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowShareButton_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSkuOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSkuOnTvChannelDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayDatePreOrderAvailability_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.UseAjaxCatalogTvChannelsLoading_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceFrom_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPagePriceTo_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SearchPageManuallyPriceRange_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.TvChannelsByTagPriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.TvChannelsByTagPriceFrom_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.TvChannelsByTagPriceTo_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.TvChannelsByTagManuallyPriceRange_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnableManufacturerFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnablePriceRangeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.EnableSpecificationAttributeFiltering_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.DisplayFromPrices_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.AttributeValueOutOfStockDisplayTypes, mo => mo.Ignore())
                .ForMember(model => model.AttributeValueOutOfStockDisplayType_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.SortOptionSearchModel, options => options.Ignore())
                .ForMember(model => model.ReviewTypeSearchModel, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.AllowUsersToSearchWithManufacturerName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowUsersToSearchWithCategoryName_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayAllPicturesOnCatalogPages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelUrlStructureTypeId_OverrideForStore, mo => mo.Ignore())
                .ForMember(model => model.TvChannelUrlStructureTypes, mo => mo.Ignore());
            CreateMap<CatalogSettingsModel, CatalogSettings>()
                .ForMember(settings => settings.AjaxProcessAttributeChange, options => options.Ignore())
                .ForMember(settings => settings.CompareTvChannelsNumber, options => options.Ignore())
                .ForMember(settings => settings.CountDisplayedYearsDatePicker, options => options.Ignore())
                .ForMember(settings => settings.DefaultCategoryPageSize, options => options.Ignore())
                .ForMember(settings => settings.DefaultCategoryPageSizeOptions, options => options.Ignore())
                .ForMember(settings => settings.DefaultManufacturerPageSize, options => options.Ignore())
                .ForMember(settings => settings.DefaultManufacturerPageSizeOptions, options => options.Ignore())
                .ForMember(settings => settings.DefaultTvChannelRatingValue, options => options.Ignore())
                .ForMember(settings => settings.DisplayTierPricesWithDiscounts, options => options.Ignore())
                .ForMember(settings => settings.ExportImportTvChannelsCountInOneFile, options => options.Ignore())
                .ForMember(settings => settings.ExportImportUseDropdownlistsForAssociatedEntities, options => options.Ignore())
                .ForMember(settings => settings.IncludeFeaturedTvChannelsInNormalLists, options => options.Ignore())
                .ForMember(settings => settings.MaximumBackInStockSubscriptions, options => options.Ignore())
                .ForMember(settings => settings.TvChannelSortingEnumDisabled, options => options.Ignore())
                .ForMember(settings => settings.TvChannelSortingEnumDisplayOrder, options => options.Ignore())
                .ForMember(settings => settings.PublishBackTvChannelWhenCancellingOrders, options => options.Ignore())
                .ForMember(settings => settings.UseAjaxLoadMenu, options => options.Ignore())
                .ForMember(settings => settings.UseLinksInRequiredTvChannelWarnings, options => options.Ignore())
                .ForMember(settings => settings.ActiveSearchProviderSystemName, options => options.Ignore());

            CreateMap<TvChannelCategory, CategoryTvChannelModel>()
                .ForMember(model => model.TvChannelName, options => options.Ignore());
            CreateMap<CategoryTvChannelModel, TvChannelCategory>()
                .ForMember(entity => entity.CategoryId, options => options.Ignore())
                .ForMember(entity => entity.TvChannelId, options => options.Ignore());

            CreateMap<Category, CategoryModel>()
                .ForMember(model => model.AvailableCategories, options => options.Ignore())
                .ForMember(model => model.AvailableCategoryTemplates, options => options.Ignore())
                .ForMember(model => model.Breadcrumb, options => options.Ignore())
                .ForMember(model => model.CategoryTvChannelSearchModel, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<CategoryModel, Category>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<CategoryTemplate, CategoryTemplateModel>();
            CreateMap<CategoryTemplateModel, CategoryTemplate>();

            CreateMap<TvChannelManufacturer, ManufacturerTvChannelModel>()
                .ForMember(model => model.TvChannelName, options => options.Ignore());
            CreateMap<ManufacturerTvChannelModel, TvChannelManufacturer>()
                .ForMember(entity => entity.ManufacturerId, options => options.Ignore())
                .ForMember(entity => entity.TvChannelId, options => options.Ignore());

            CreateMap<Manufacturer, ManufacturerModel>()
                .ForMember(model => model.AvailableManufacturerTemplates, options => options.Ignore())
                .ForMember(model => model.ManufacturerTvChannelSearchModel, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<ManufacturerModel, Manufacturer>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ManufacturerTemplate, ManufacturerTemplateModel>();
            CreateMap<ManufacturerTemplateModel, ManufacturerTemplate>();

            //Review type
            CreateMap<ReviewType, ReviewTypeModel>();
            CreateMap<ReviewTypeModel, ReviewType>();

            //tvchannel review
            CreateMap<TvChannelReview, TvChannelReviewModel>()
                .ForMember(model => model.UserInfo, mo => mo.Ignore())
                .ForMember(model => model.IsLoggedInAsVendor, mo => mo.Ignore())
                .ForMember(model => model.TvChannelReviewReviewTypeMappingSearchModel, mo => mo.Ignore())
                .ForMember(model => model.CreatedOn, mo => mo.Ignore())
                .ForMember(model => model.StoreName, mo => mo.Ignore())
                .ForMember(model => model.ShowStoreName, mo => mo.Ignore())
                .ForMember(model => model.TvChannelName, mo => mo.Ignore());

            //tvchannel review type mapping
            CreateMap<TvChannelReviewReviewTypeMapping, TvChannelReviewReviewTypeMappingModel>()
                .ForMember(model => model.Name, mo => mo.Ignore())
                .ForMember(model => model.Description, mo => mo.Ignore())
                .ForMember(model => model.VisibleToAllUsers, mo => mo.Ignore());

            //tvchannels
            CreateMap<TvChannel, TvChannelModel>()
                .ForMember(model => model.AddPictureModel, options => options.Ignore())
                .ForMember(model => model.AssociatedTvChannelSearchModel, options => options.Ignore())
                .ForMember(model => model.AssociatedToTvChannelId, options => options.Ignore())
                .ForMember(model => model.AssociatedToTvChannelName, options => options.Ignore())
                .ForMember(model => model.AvailableBasepriceBaseUnits, options => options.Ignore())
                .ForMember(model => model.AvailableBasepriceUnits, options => options.Ignore())
                .ForMember(model => model.AvailableCategories, options => options.Ignore())
                .ForMember(model => model.AvailableDeliveryDates, options => options.Ignore())
                .ForMember(model => model.AvailableManufacturers, options => options.Ignore())
                .ForMember(model => model.AvailableTvChannelAvailabilityRanges, options => options.Ignore())
                .ForMember(model => model.AvailableTvChannelTemplates, options => options.Ignore())
                .ForMember(model => model.AvailableTaxCategories, options => options.Ignore())
                .ForMember(model => model.AvailableVendors, options => options.Ignore())
                .ForMember(model => model.AvailableWarehouses, options => options.Ignore())
                .ForMember(model => model.BaseDimensionIn, options => options.Ignore())
                .ForMember(model => model.BaseWeightIn, options => options.Ignore())
                .ForMember(model => model.CopyTvChannelModel, options => options.Ignore())
                .ForMember(model => model.CrossSellTvChannelSearchModel, options => options.Ignore())
                .ForMember(model => model.HasAvailableSpecificationAttributes, options => options.Ignore())
                .ForMember(model => model.IsLoggedInAsVendor, options => options.Ignore())
                .ForMember(model => model.LastStockQuantity, options => options.Ignore())
                .ForMember(model => model.PictureThumbnailUrl, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.TvChannelAttributeCombinationSearchModel, options => options.Ignore())
                .ForMember(model => model.TvChannelAttributeMappingSearchModel, options => options.Ignore())
                .ForMember(model => model.TvChannelAttributesExist, options => options.Ignore())
                .ForMember(model => model.CanCreateCombinations, options => options.Ignore())
                .ForMember(model => model.TvChannelEditorSettingsModel, options => options.Ignore())
                .ForMember(model => model.TvChannelOrderSearchModel, options => options.Ignore())
                .ForMember(model => model.TvChannelPictureModels, options => options.Ignore())
                .ForMember(model => model.TvChannelPictureSearchModel, options => options.Ignore())
                .ForMember(model => model.TvChannelVideoModels, options => options.Ignore())
                .ForMember(model => model.TvChannelVideoSearchModel, options => options.Ignore())
                .ForMember(model => model.AddVideoModel, options => options.Ignore())
                .ForMember(model => model.TvChannelSpecificationAttributeSearchModel, options => options.Ignore())
                .ForMember(model => model.TvChannelsTypesSupportedByTvChannelTemplates, options => options.Ignore())
                .ForMember(model => model.TvChannelTags, options => options.Ignore())
                .ForMember(model => model.TvChannelTypeName, options => options.Ignore())
                .ForMember(model => model.TvChannelWarehouseInventoryModels, options => options.Ignore())
                .ForMember(model => model.RelatedTvChannelSearchModel, options => options.Ignore())
                .ForMember(model => model.SelectedCategoryIds, options => options.Ignore())
                .ForMember(model => model.SelectedManufacturerIds, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.StockQuantityHistory, options => options.Ignore())
                .ForMember(model => model.StockQuantityHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.StockQuantityStr, options => options.Ignore())
                .ForMember(model => model.TierPriceSearchModel, options => options.Ignore())
                .ForMember(model => model.InitialTvChannelTags, options => options.Ignore());
            CreateMap<TvChannelModel, TvChannel>()
                .ForMember(entity => entity.ApprovedRatingSum, options => options.Ignore())
                .ForMember(entity => entity.ApprovedTotalReviews, options => options.Ignore())
                .ForMember(entity => entity.BackorderMode, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.DownloadActivationType, options => options.Ignore())
                .ForMember(entity => entity.GiftCardType, options => options.Ignore())
                .ForMember(entity => entity.HasDiscountsApplied, options => options.Ignore())
                .ForMember(entity => entity.HasTierPrices, options => options.Ignore())
                .ForMember(entity => entity.LowStockActivity, options => options.Ignore())
                .ForMember(entity => entity.ManageInventoryMethod, options => options.Ignore())
                .ForMember(entity => entity.NotApprovedRatingSum, options => options.Ignore())
                .ForMember(entity => entity.NotApprovedTotalReviews, options => options.Ignore())
                .ForMember(entity => entity.ParentGroupedTvChannelId, options => options.Ignore())
                .ForMember(entity => entity.TvChannelType, options => options.Ignore())
                .ForMember(entity => entity.RecurringCyclePeriod, options => options.Ignore())
                .ForMember(entity => entity.RentalPricePeriod, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<TvChannel, DiscountTvChannelModel>()
                .ForMember(model => model.TvChannelId, options => options.Ignore())
                .ForMember(model => model.TvChannelName, options => options.Ignore());

            CreateMap<TvChannel, AssociatedTvChannelModel>()
                .ForMember(model => model.TvChannelName, options => options.Ignore());

            CreateMap<TvChannelAttributeCombination, TvChannelAttributeCombinationModel>()
               .ForMember(model => model.AttributesXml, options => options.Ignore())
               .ForMember(model => model.TvChannelAttributes, options => options.Ignore())
               .ForMember(model => model.TvChannelPictureModels, options => options.Ignore())
               .ForMember(model => model.PictureThumbnailUrl, options => options.Ignore())
               .ForMember(model => model.Warnings, options => options.Ignore());
            CreateMap<TvChannelAttributeCombinationModel, TvChannelAttributeCombination>()
               .ForMember(entity => entity.AttributesXml, options => options.Ignore());

            CreateMap<TvChannelAttribute, TvChannelAttributeModel>()
                .ForMember(model => model.PredefinedTvChannelAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.TvChannelAttributeTvChannelSearchModel, options => options.Ignore());
            CreateMap<TvChannelAttributeModel, TvChannelAttribute>();

            CreateMap<TvChannel, TvChannelAttributeTvChannelModel>()
                .ForMember(model => model.TvChannelName, options => options.Ignore());

            CreateMap<PredefinedTvChannelAttributeValue, PredefinedTvChannelAttributeValueModel>()
                .ForMember(model => model.WeightAdjustmentStr, options => options.Ignore())
                .ForMember(model => model.PriceAdjustmentStr, options => options.Ignore());
            CreateMap<PredefinedTvChannelAttributeValueModel, PredefinedTvChannelAttributeValue>();

            CreateMap<TvChannelAttributeMapping, TvChannelAttributeMappingModel>()
                .ForMember(model => model.ValidationRulesString, options => options.Ignore())
                .ForMember(model => model.AttributeControlType, options => options.Ignore())
                .ForMember(model => model.ConditionString, options => options.Ignore())
                .ForMember(model => model.TvChannelAttribute, options => options.Ignore())
                .ForMember(model => model.AvailableTvChannelAttributes, options => options.Ignore())
                .ForMember(model => model.ConditionAllowed, options => options.Ignore())
                .ForMember(model => model.ConditionModel, options => options.Ignore())
                .ForMember(model => model.TvChannelAttributeValueSearchModel, options => options.Ignore());
            CreateMap<TvChannelAttributeMappingModel, TvChannelAttributeMapping>()
                .ForMember(entity => entity.ConditionAttributeXml, options => options.Ignore())
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<TvChannelAttributeValue, TvChannelAttributeValueModel>()
                .ForMember(model => model.AttributeValueTypeName, options => options.Ignore())
                .ForMember(model => model.Name, options => options.Ignore())
                .ForMember(model => model.PriceAdjustmentStr, options => options.Ignore())
                .ForMember(model => model.AssociatedTvChannelName, options => options.Ignore())
                .ForMember(model => model.PictureThumbnailUrl, options => options.Ignore())
                .ForMember(model => model.WeightAdjustmentStr, options => options.Ignore())
                .ForMember(model => model.DisplayColorSquaresRgb, options => options.Ignore())
                .ForMember(model => model.DisplayImageSquaresPicture, options => options.Ignore())
                .ForMember(model => model.TvChannelPictureModels, options => options.Ignore());
            CreateMap<TvChannelAttributeValueModel, TvChannelAttributeValue>()
               .ForMember(entity => entity.AttributeValueType, options => options.Ignore())
               .ForMember(entity => entity.Quantity, options => options.Ignore());

            CreateMap<TvChannelEditorSettings, TvChannelEditorSettingsModel>();
            CreateMap<TvChannelEditorSettingsModel, TvChannelEditorSettings>();

            CreateMap<TvChannelPicture, TvChannelPictureModel>()
                .ForMember(model => model.OverrideAltAttribute, options => options.Ignore())
                .ForMember(model => model.OverrideTitleAttribute, options => options.Ignore())
                .ForMember(model => model.PictureUrl, options => options.Ignore());

            CreateMap<TvChannelVideo, TvChannelVideoModel>()
               .ForMember(model => model.VideoUrl, options => options.Ignore());

            CreateMap<TvChannel, SpecificationAttributeTvChannelModel>()
                .ForMember(model => model.SpecificationAttributeId, options => options.Ignore())
                .ForMember(model => model.TvChannelId, options => options.Ignore())
                .ForMember(model => model.TvChannelName, options => options.Ignore());

            CreateMap<TvChannelSpecificationAttribute, TvChannelSpecificationAttributeModel>()
                .ForMember(model => model.AttributeTypeName, options => options.Ignore())
                .ForMember(model => model.ValueRaw, options => options.Ignore())
                .ForMember(model => model.AttributeId, options => options.Ignore())
                .ForMember(model => model.AttributeName, options => options.Ignore())
                .ForMember(model => model.SpecificationAttributeOptionId, options => options.Ignore());

            CreateMap<TvChannelSpecificationAttribute, AddSpecificationAttributeModel>()
                .ForMember(entity => entity.SpecificationId, options => options.Ignore())
                .ForMember(entity => entity.AttributeTypeName, options => options.Ignore())
                .ForMember(entity => entity.AttributeId, options => options.Ignore())
                .ForMember(entity => entity.AttributeName, options => options.Ignore())
                .ForMember(entity => entity.ValueRaw, options => options.Ignore())
                .ForMember(entity => entity.Value, options => options.Ignore())
                .ForMember(entity => entity.AvailableOptions, options => options.Ignore())
                .ForMember(entity => entity.AvailableAttributes, options => options.Ignore());

            CreateMap<AddSpecificationAttributeModel, TvChannelSpecificationAttribute>()
                .ForMember(model => model.CustomValue, options => options.Ignore())
                .ForMember(model => model.AttributeType, options => options.Ignore());

            CreateMap<TvChannelTag, TvChannelTagModel>()
               .ForMember(model => model.TvChannelCount, options => options.Ignore());

            CreateMap<TvChannelTemplate, TvChannelTemplateModel>();
            CreateMap<TvChannelTemplateModel, TvChannelTemplate>();

            CreateMap<RelatedTvChannel, RelatedTvChannelModel>()
               .ForMember(model => model.TvChannel2Name, options => options.Ignore());

            CreateMap<SpecificationAttribute, SpecificationAttributeModel>()
                .ForMember(model => model.SpecificationAttributeOptionSearchModel, options => options.Ignore())
                .ForMember(model => model.SpecificationAttributeTvChannelSearchModel, options => options.Ignore())
                .ForMember(model => model.AvailableGroups, options => options.Ignore());
            CreateMap<SpecificationAttributeModel, SpecificationAttribute>();

            CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionModel>()
                .ForMember(model => model.EnableColorSquaresRgb, options => options.Ignore())
                .ForMember(model => model.NumberOfAssociatedTvChannels, options => options.Ignore());
            CreateMap<SpecificationAttributeOptionModel, SpecificationAttributeOption>();

            CreateMap<SpecificationAttributeGroup, SpecificationAttributeGroupModel>();
            CreateMap<SpecificationAttributeGroupModel, SpecificationAttributeGroup>();

            CreateMap<StockQuantityHistory, StockQuantityHistoryModel>()
                .ForMember(model => model.WarehouseName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.AttributeCombination, options => options.Ignore());

            CreateMap<TierPrice, TierPriceModel>()
                .ForMember(model => model.Store, options => options.Ignore())
                .ForMember(model => model.AvailableUserRoles, options => options.Ignore())
                .ForMember(model => model.AvailableStores, options => options.Ignore())
                .ForMember(model => model.UserRole, options => options.Ignore());
            CreateMap<TierPriceModel, TierPrice>()
                .ForMember(entity => entity.UserRoleId, options => options.Ignore())
                .ForMember(entity => entity.TvChannelId, options => options.Ignore());
        }

        /// <summary>
        /// Create CMS maps 
        /// </summary>
        protected virtual void CreateCmsMaps()
        {
            CreateMap<IWidgetPlugin, WidgetModel>()
                .ForMember(model => model.WidgetViewComponentArguments, options => options.Ignore())
                .ForMember(model => model.WidgetViewComponentName, options => options.Ignore());
        }

        /// <summary>
        /// Create common maps 
        /// </summary>
        protected virtual void CreateCommonMaps()
        {
            CreateMap<Address, AddressModel>()
                .ForMember(model => model.AddressHtml, options => options.Ignore())
                .ForMember(model => model.AvailableCountries, options => options.Ignore())
                .ForMember(model => model.AvailableStates, options => options.Ignore())
                .ForMember(model => model.CountryName, options => options.Ignore())
                .ForMember(model => model.CustomAddressAttributes, options => options.Ignore())
                .ForMember(model => model.FormattedCustomAddressAttributes, options => options.Ignore())
                .ForMember(model => model.StateProvinceName, options => options.Ignore())
                .ForMember(model => model.CityRequired, options => options.Ignore())
                .ForMember(model => model.CompanyRequired, options => options.Ignore())
                .ForMember(model => model.CountryRequired, options => options.Ignore())
                .ForMember(model => model.CountyRequired, options => options.Ignore())
                .ForMember(model => model.EmailRequired, options => options.Ignore())
                .ForMember(model => model.FaxRequired, options => options.Ignore())
                .ForMember(model => model.FirstNameRequired, options => options.Ignore())
                .ForMember(model => model.LastNameRequired, options => options.Ignore())
                .ForMember(model => model.SmartPhoneRequired, options => options.Ignore())
                .ForMember(model => model.StateProvinceName, options => options.Ignore())
                .ForMember(model => model.StreetAddress2Required, options => options.Ignore())
                .ForMember(model => model.StreetAddressRequired, options => options.Ignore())
                .ForMember(model => model.ZipPostalCodeRequired, options => options.Ignore());
            CreateMap<AddressModel, Address>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CustomAttributes, options => options.Ignore());

            CreateMap<AddressAttribute, AddressAttributeModel>()
                .ForMember(model => model.AddressAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore());
            CreateMap<AddressAttributeModel, AddressAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<AddressAttributeValue, AddressAttributeValueModel>();
            CreateMap<AddressAttributeValueModel, AddressAttributeValue>();

            CreateMap<AddressSettings, AddressSettingsModel>();
            CreateMap<AddressSettingsModel, AddressSettings>()
                .ForMember(settings => settings.PreselectCountryIfOnlyOne, options => options.Ignore());

            CreateMap<Setting, SettingModel>()
                .ForMember(setting => setting.AvailableStores, options => options.Ignore())
                .ForMember(setting => setting.Store, options => options.Ignore());
        }

        /// <summary>
        /// Create users maps 
        /// </summary>
        protected virtual void CreateUsersMaps()
        {
            CreateMap<UserAttribute, UserAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.UserAttributeValueSearchModel, options => options.Ignore());
            CreateMap<UserAttributeModel, UserAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<UserAttributeValue, UserAttributeValueModel>();
            CreateMap<UserAttributeValueModel, UserAttributeValue>();

            CreateMap<UserRole, UserRoleModel>()
                .ForMember(model => model.PurchasedWithTvChannelName, options => options.Ignore())
                .ForMember(model => model.TaxDisplayTypeValues, options => options.Ignore());
            CreateMap<UserRoleModel, UserRole>();

            CreateMap<UserSettings, UserSettingsModel>();
            CreateMap<UserSettingsModel, UserSettings>()
                .ForMember(settings => settings.AvatarMaximumSizeBytes, options => options.Ignore())
                .ForMember(settings => settings.DeleteGuestTaskOlderThanMinutes, options => options.Ignore())
                .ForMember(settings => settings.DownloadableTvChannelsValidateUser, options => options.Ignore())
                .ForMember(settings => settings.HashedPasswordFormat, options => options.Ignore())
                .ForMember(settings => settings.OnlineUserMinutes, options => options.Ignore())
                .ForMember(settings => settings.SuffixDeletedUsers, options => options.Ignore())
                .ForMember(settings => settings.LastActivityMinutes, options => options.Ignore())
                .ForMember(settings => settings.AcceptPersonalDataAgreement, options => options.Ignore());

            CreateMap<MultiFactorAuthenticationSettings, MultiFactorAuthenticationSettingsModel>();
            CreateMap<MultiFactorAuthenticationSettingsModel, MultiFactorAuthenticationSettings>()
                .ForMember(settings => settings.ActiveAuthenticationMethodSystemNames, option => option.Ignore());

            CreateMap<RewardPointsSettings, RewardPointsSettingsModel>()
                .ForMember(model => model.ActivatePointsImmediately, options => options.Ignore())
                .ForMember(model => model.ActivationDelay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayHowMuchWillBeEarned_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExchangeRate_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumRewardPointsToUsePerOrder_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinimumRewardPointsToUse_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinOrderTotalToAwardPoints_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PointsForPurchases_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumRedeemedRate_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PointsForRegistration_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.PurchasesPointsValidity_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RegistrationPointsValidity_OverrideForStore, options => options.Ignore());
            CreateMap<RewardPointsSettingsModel, RewardPointsSettings>();

            CreateMap<RewardPointsHistory, UserRewardPointsModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.PointsBalance, options => options.Ignore())
                .ForMember(model => model.EndDate, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());

            CreateMap<ActivityLog, UserActivityLogModel>()
               .ForMember(model => model.CreatedOn, options => options.Ignore())
               .ForMember(model => model.ActivityLogTypeName, options => options.Ignore());

            CreateMap<User, UserModel>()
                .ForMember(model => model.Email, options => options.Ignore())
                .ForMember(model => model.FullName, options => options.Ignore())
                .ForMember(model => model.Company, options => options.Ignore())
                .ForMember(model => model.SmartPhone, options => options.Ignore())
                .ForMember(model => model.ZipPostalCode, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.UserRoleNames, options => options.Ignore())
                .ForMember(model => model.AvatarUrl, options => options.Ignore())
                .ForMember(model => model.UsernamesEnabled, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.AvailableVendors, options => options.Ignore())
                .ForMember(model => model.GenderEnabled, options => options.Ignore())
                .ForMember(model => model.Gender, options => options.Ignore())
                .ForMember(model => model.FirstNameEnabled, options => options.Ignore())
                .ForMember(model => model.FirstName, options => options.Ignore())
                .ForMember(model => model.LastNameEnabled, options => options.Ignore())
                .ForMember(model => model.LastName, options => options.Ignore())
                .ForMember(model => model.MiddleNameEnabled, options => options.Ignore())
                .ForMember(model => model.MiddleName, options => options.Ignore())
                .ForMember(model => model.BirthDateEnabled, options => options.Ignore())
                .ForMember(model => model.BirthDate, options => options.Ignore())
                .ForMember(model => model.CompanyEnabled, options => options.Ignore())
                .ForMember(model => model.StreetAddressEnabled, options => options.Ignore())
                .ForMember(model => model.StreetAddress, options => options.Ignore())
                .ForMember(model => model.StreetAddress2Enabled, options => options.Ignore())
                .ForMember(model => model.StreetAddress2, options => options.Ignore())
                .ForMember(model => model.ZipPostalCodeEnabled, options => options.Ignore())
                .ForMember(model => model.CityEnabled, options => options.Ignore())
                .ForMember(model => model.City, options => options.Ignore())
                .ForMember(model => model.CountyEnabled, options => options.Ignore())
                .ForMember(model => model.County, options => options.Ignore())
                .ForMember(model => model.CountryEnabled, options => options.Ignore())
                .ForMember(model => model.CountryId, options => options.Ignore())
                .ForMember(model => model.AvailableCountries, options => options.Ignore())
                .ForMember(model => model.StateProvinceEnabled, options => options.Ignore())
                .ForMember(model => model.StateProvinceId, options => options.Ignore())
                .ForMember(model => model.AvailableStates, options => options.Ignore())
                .ForMember(model => model.SmartPhoneEnabled, options => options.Ignore())
                .ForMember(model => model.FaxEnabled, options => options.Ignore())
                .ForMember(model => model.Fax, options => options.Ignore())
                .ForMember(model => model.UserAttributes, options => options.Ignore())
                .ForMember(model => model.RegisteredInStore, options => options.Ignore())
                .ForMember(model => model.DisplayRegisteredInStore, options => options.Ignore())
                .ForMember(model => model.AffiliateName, options => options.Ignore())
                .ForMember(model => model.GmtZone, options => options.Ignore())
                .ForMember(model => model.AllowUsersToSetTimeZone, options => options.Ignore())
                .ForMember(model => model.AvailableTimeZones, options => options.Ignore())
                .ForMember(model => model.VatNumber, options => options.Ignore())
                .ForMember(model => model.VatNumberStatusNote, options => options.Ignore())
                .ForMember(model => model.DisplayVatNumber, options => options.Ignore())
                .ForMember(model => model.LastVisitedPage, options => options.Ignore())
                .ForMember(model => model.AvailableNewsletterSubscriptionStores, options => options.Ignore())
                .ForMember(model => model.SelectedNewsletterSubscriptionStoreIds, options => options.Ignore())
                .ForMember(model => model.DisplayRewardPointsHistory, options => options.Ignore())
                .ForMember(model => model.AddRewardPoints, options => options.Ignore())
                .ForMember(model => model.UserRewardPointsSearchModel, options => options.Ignore())
                .ForMember(model => model.SendEmail, options => options.Ignore())
                .ForMember(model => model.SendPm, options => options.Ignore())
                .ForMember(model => model.AllowSendingOfPrivateMessage, options => options.Ignore())
                .ForMember(model => model.AllowSendingOfWelcomeMessage, options => options.Ignore())
                .ForMember(model => model.AllowReSendingOfActivationMessage, options => options.Ignore())
                .ForMember(model => model.GdprEnabled, options => options.Ignore())
                .ForMember(model => model.MultiFactorAuthenticationProvider, options => options.Ignore())
                .ForMember(model => model.UserAssociatedExternalAuthRecordsSearchModel, options => options.Ignore())
                .ForMember(model => model.UserAddressSearchModel, options => options.Ignore())
                .ForMember(model => model.UserOrderSearchModel, options => options.Ignore())
                .ForMember(model => model.UserShoppingCartSearchModel, options => options.Ignore())
                .ForMember(model => model.UserActivityLogSearchModel, options => options.Ignore())
                .ForMember(model => model.UserBackInStockSubscriptionSearchModel, options => options.Ignore());

            CreateMap<UserModel, User>()
                .ForMember(entity => entity.UserGuid, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LastActivityDateUtc, options => options.Ignore())
                .ForMember(entity => entity.EmailToRevalidate, options => options.Ignore())
                .ForMember(entity => entity.HasShoppingCartItems, options => options.Ignore())
                .ForMember(entity => entity.RequireReLogin, options => options.Ignore())
                .ForMember(entity => entity.FailedLoginAttempts, options => options.Ignore())
                .ForMember(entity => entity.CannotLoginUntilDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.IsSystemAccount, options => options.Ignore())
                .ForMember(entity => entity.SystemName, options => options.Ignore())
                .ForMember(entity => entity.LastLoginDateUtc, options => options.Ignore())
                .ForMember(entity => entity.BillingAddressId, options => options.Ignore())
                .ForMember(entity => entity.ShippingAddressId, options => options.Ignore())
                .ForMember(entity => entity.VatNumberStatusId, options => options.Ignore())
                .ForMember(entity => entity.CustomUserAttributesXML, options => options.Ignore())
                .ForMember(entity => entity.CurrencyId, options => options.Ignore())
                .ForMember(entity => entity.LanguageId, options => options.Ignore())
                .ForMember(entity => entity.TaxDisplayTypeId, options => options.Ignore())
                .ForMember(entity => entity.VatNumberStatus, options => options.Ignore())
                .ForMember(entity => entity.TaxDisplayType, options => options.Ignore())
                .ForMember(entity => entity.RegisteredInStoreId, options => options.Ignore());

            CreateMap<User, OnlineUserModel>()
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.UserInfo, options => options.Ignore())
                .ForMember(model => model.LastIpAddress, options => options.Ignore())
                .ForMember(model => model.Location, options => options.Ignore())
                .ForMember(model => model.LastVisitedPage, options => options.Ignore());

            CreateMap<BackInStockSubscription, UserBackInStockSubscriptionModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore())
                .ForMember(model => model.TvChannelName, options => options.Ignore());
        }

        /// <summary>
        /// Create directory maps 
        /// </summary>
        protected virtual void CreateDirectoryMaps()
        {
            CreateMap<Country, CountryModel>()
                .ForMember(model => model.NumberOfStates, options => options.Ignore())
                .ForMember(model => model.StateProvinceSearchModel, options => options.Ignore());
            CreateMap<CountryModel, Country>();

            CreateMap<Currency, CurrencyModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.IsPrimaryExchangeRateCurrency, options => options.Ignore())
                .ForMember(model => model.IsPrimaryStoreCurrency, options => options.Ignore());
            CreateMap<CurrencyModel, Currency>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.RoundingType, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<MeasureDimension, MeasureDimensionModel>()
                .ForMember(model => model.IsPrimaryDimension, options => options.Ignore());
            CreateMap<MeasureDimensionModel, MeasureDimension>();

            CreateMap<MeasureWeight, MeasureWeightModel>()
                .ForMember(model => model.IsPrimaryWeight, options => options.Ignore());
            CreateMap<MeasureWeightModel, MeasureWeight>();

            CreateMap<StateProvince, StateProvinceModel>();
            CreateMap<StateProvinceModel, StateProvince>();
        }

        /// <summary>
        /// Create discounts maps 
        /// </summary>
        protected virtual void CreateDiscountsMaps()
        {
            CreateMap<Discount, DiscountModel>()
                .ForMember(model => model.AddDiscountRequirement, options => options.Ignore())
                .ForMember(model => model.AvailableDiscountRequirementRules, options => options.Ignore())
                .ForMember(model => model.AvailableRequirementGroups, options => options.Ignore())
                .ForMember(model => model.DiscountCategorySearchModel, options => options.Ignore())
                .ForMember(model => model.DiscountManufacturerSearchModel, options => options.Ignore())
                .ForMember(model => model.DiscountTvChannelSearchModel, options => options.Ignore())
                .ForMember(model => model.DiscountTypeName, options => options.Ignore())
                .ForMember(model => model.DiscountUrl, options => options.Ignore())
                .ForMember(model => model.DiscountUsageHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.GroupName, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.RequirementGroupId, options => options.Ignore())
                .ForMember(model => model.TimesUsed, options => options.Ignore());
            CreateMap<DiscountModel, Discount>()
                .ForMember(entity => entity.DiscountLimitation, options => options.Ignore())
                .ForMember(entity => entity.DiscountType, options => options.Ignore());

            CreateMap<DiscountUsageHistory, DiscountUsageHistoryModel>()
                .ForMember(entity => entity.CreatedOn, options => options.Ignore())
                .ForMember(entity => entity.OrderTotal, options => options.Ignore())
                .ForMember(entity => entity.CustomOrderNumber, options => options.Ignore());

            CreateMap<Category, DiscountCategoryModel>()
                .ForMember(entity => entity.CategoryName, options => options.Ignore())
                .ForMember(entity => entity.CategoryId, options => options.Ignore());

            CreateMap<Manufacturer, DiscountManufacturerModel>()
                .ForMember(entity => entity.ManufacturerId, options => options.Ignore())
                .ForMember(entity => entity.ManufacturerName, options => options.Ignore());
        }

        /// <summary>
        /// Create forums maps 
        /// </summary>
        protected virtual void CreateForumsMaps()
        {
            CreateMap<Forum, ForumModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.ForumGroups, options => options.Ignore());
            CreateMap<ForumModel, Forum>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LastPostUserId, options => options.Ignore())
                .ForMember(entity => entity.LastPostId, options => options.Ignore())
                .ForMember(entity => entity.LastPostTime, options => options.Ignore())
                .ForMember(entity => entity.LastTopicId, options => options.Ignore())
                .ForMember(entity => entity.NumPosts, options => options.Ignore())
                .ForMember(entity => entity.NumTopics, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ForumGroup, ForumGroupModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore());
            CreateMap<ForumGroupModel, ForumGroup>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ForumSettings, ForumSettingsModel>()
                .ForMember(model => model.ActiveDiscussionsFeedCount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ActiveDiscussionsFeedEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ActiveDiscussionsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowUsersToDeletePosts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowUsersToEditPosts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowUsersToManageSubscriptions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowGuestsToCreatePosts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowGuestsToCreateTopics_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowPostVoting_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowPrivateMessages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForumEditorValues, options => options.Ignore())
                .ForMember(model => model.ForumEditor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForumFeedCount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForumFeedsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForumsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaxVotesPerDay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyAboutPrivateMessages_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PostsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.RelativeDateTimeFormattingEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SearchResultsPageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowAlertForPM_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowUsersPostCount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SignaturesEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TopicsPageSize_OverrideForStore, options => options.Ignore());
            CreateMap<ForumSettingsModel, ForumSettings>()
                .ForMember(settings => settings.ForumSearchTermMinimumLength, options => options.Ignore())
                .ForMember(settings => settings.ForumSubscriptionsPageSize, options => options.Ignore())
                .ForMember(settings => settings.HomepageActiveDiscussionsTopicCount, options => options.Ignore())
                .ForMember(settings => settings.LatestUserPostsPageSize, options => options.Ignore())
                .ForMember(settings => settings.PMSubjectMaxLength, options => options.Ignore())
                .ForMember(settings => settings.PMTextMaxLength, options => options.Ignore())
                .ForMember(settings => settings.PostMaxLength, options => options.Ignore())
                .ForMember(settings => settings.PrivateMessagesPageSize, options => options.Ignore())
                .ForMember(settings => settings.StrippedTopicMaxLength, options => options.Ignore())
                .ForMember(settings => settings.TopicSubjectMaxLength, options => options.Ignore());
        }

        /// <summary>
        /// Create GDPR maps 
        /// </summary>
        protected virtual void CreateGdprMaps()
        {
            CreateMap<GdprSettings, GdprSettingsModel>()
                .ForMember(model => model.GdprConsentSearchModel, options => options.Ignore())
                .ForMember(model => model.GdprEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LogNewsletterConsent_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LogPrivacyPolicyConsent_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LogUserProfileChanges_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DeleteInactiveUsersAfterMonths_OverrideForStore, options => options.Ignore());
            CreateMap<GdprSettingsModel, GdprSettings>();

            CreateMap<GdprConsent, GdprConsentModel>();
            CreateMap<GdprConsentModel, GdprConsent>();

            CreateMap<GdprLog, GdprLogModel>()
                .ForMember(model => model.UserInfo, options => options.Ignore())
                .ForMember(model => model.RequestType, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());
        }

        /// <summary>
        /// Create localization maps 
        /// </summary>
        protected virtual void CreateLocalizationMaps()
        {
            CreateMap<Language, LanguageModel>()
                .ForMember(model => model.AvailableCurrencies, options => options.Ignore())
                .ForMember(model => model.LocaleResourceSearchModel, options => options.Ignore());
            CreateMap<LanguageModel, Language>();

            CreateMap<LocaleResourceModel, LocaleStringResource>()
                .ForMember(entity => entity.LanguageId, options => options.Ignore());
        }

        /// <summary>
        /// Create logging maps 
        /// </summary>
        protected virtual void CreateLoggingMaps()
        {
            CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(model => model.ActivityLogTypeName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.UserEmail, options => options.Ignore());
            CreateMap<ActivityLogModel, ActivityLog>()
                .ForMember(entity => entity.ActivityLogTypeId, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.EntityId, options => options.Ignore())
                .ForMember(entity => entity.EntityName, options => options.Ignore());

            CreateMap<ActivityLogType, ActivityLogTypeModel>();
            CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(entity => entity.SystemKeyword, options => options.Ignore());

            CreateMap<Log, LogModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.FullMessage, options => options.Ignore())
                .ForMember(model => model.UserEmail, options => options.Ignore());
            CreateMap<LogModel, Log>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LogLevelId, options => options.Ignore());
        }

        /// <summary>
        /// Create media maps 
        /// </summary>
        protected virtual void CreateMediaMaps()
        {
            CreateMap<MediaSettings, MediaSettingsModel>()
                .ForMember(model => model.AssociatedTvChannelPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AvatarPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CartThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OrderThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CategoryThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultImageQuality_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultPictureZoomEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ImportTvChannelImagesUsingHash_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ManufacturerThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumImageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MiniCartThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MultipleThumbDirectories_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PicturesStoredIntoDatabase, options => options.Ignore())
                .ForMember(model => model.TvChannelDetailsPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelThumbPictureSizeOnTvChannelDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.VendorThumbPictureSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TvChannelDefaultImageId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowSVGUploads_OverrideForStore, options => options.Ignore());
            CreateMap<MediaSettingsModel, MediaSettings>()
                .ForMember(settings => settings.AutoCompleteSearchThumbPictureSize, options => options.Ignore())
                .ForMember(settings => settings.AzureCacheControlHeader, options => options.Ignore())
                .ForMember(settings => settings.UseAbsoluteImagePath, options => options.Ignore())
                .ForMember(settings => settings.ImageSquarePictureSize, options => options.Ignore())
                .ForMember(settings => settings.VideoIframeAllow, options => options.Ignore())
                .ForMember(settings => settings.VideoIframeHeight, options => options.Ignore())
                .ForMember(settings => settings.VideoIframeWidth, options => options.Ignore());
        }

        /// <summary>
        /// Create messages maps 
        /// </summary>
        protected virtual void CreateMessagesMaps()
        {
            CreateMap<Campaign, CampaignModel>()
                .ForMember(model => model.AllowedTokens, options => options.Ignore())
                .ForMember(model => model.AvailableUserRoles, options => options.Ignore())
                .ForMember(model => model.AvailableEmailAccounts, options => options.Ignore())
                .ForMember(model => model.AvailableStores, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
                .ForMember(model => model.EmailAccountId, options => options.Ignore())
                .ForMember(model => model.TestEmail, options => options.Ignore());
            CreateMap<CampaignModel, Campaign>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore());

            CreateMap<EmailAccount, EmailAccountModel>()
                .ForMember(model => model.IsDefaultEmailAccount, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.SendTestEmailTo, options => options.Ignore());
            CreateMap<EmailAccountModel, EmailAccount>()
                .ForMember(entity => entity.Password, options => options.Ignore());

            CreateMap<MessageTemplate, MessageTemplateModel>()
                .ForMember(model => model.AllowedTokens, options => options.Ignore())
                .ForMember(model => model.AvailableEmailAccounts, options => options.Ignore())
                .ForMember(model => model.HasAttachedDownload, options => options.Ignore())
                .ForMember(model => model.ListOfStores, options => options.Ignore())
                .ForMember(model => model.SendImmediately, options => options.Ignore());
            CreateMap<MessageTemplateModel, MessageTemplate>()
                .ForMember(entity => entity.DelayPeriod, options => options.Ignore());

            CreateMap<NewsLetterSubscription, NewsletterSubscriptionModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());
            CreateMap<NewsletterSubscriptionModel, NewsLetterSubscription>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.NewsLetterSubscriptionGuid, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore());

            CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
                .ForMember(model => model.EmailAccountName, options => options.Ignore())
                .ForMember(model => model.PriorityName, options => options.Ignore())
                .ForMember(model => model.SendImmediately, options => options.Ignore())
                .ForMember(model => model.SentOn, options => options.Ignore());
            CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(entity => entity.AttachmentFileName, options => options.Ignore())
                .ForMember(entity => entity.AttachmentFilePath, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore())
                .ForMember(entity => entity.EmailAccountId, options => options.Ignore())
                .ForMember(entity => entity.Priority, options => options.Ignore())
                .ForMember(entity => entity.PriorityId, options => options.Ignore())
                .ForMember(entity => entity.SentOnUtc, options => options.Ignore());
        }

        /// <summary>
        /// Create news maps 
        /// </summary>
        protected virtual void CreateNewsMaps()
        {
            CreateMap<NewsComment, NewsCommentModel>()
                .ForMember(model => model.UserInfo, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CommentText, options => options.Ignore())
                .ForMember(model => model.NewsItemTitle, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());
            CreateMap<NewsCommentModel, NewsComment>()
                .ForMember(entity => entity.CommentTitle, options => options.Ignore())
                .ForMember(entity => entity.CommentText, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.NewsItemId, options => options.Ignore())
                .ForMember(entity => entity.UserId, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore());

            CreateMap<NewsItem, NewsItemModel>()
                .ForMember(model => model.ApprovedComments, options => options.Ignore())
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LanguageName, options => options.Ignore())
                .ForMember(model => model.NotApprovedComments, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore());
            CreateMap<NewsItemModel, NewsItem>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

            CreateMap<NewsSettings, NewsSettingsModel>()
                .ForMember(model => model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MainPageNewsCount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewsArchivePageSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewsCommentsMustBeApproved_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyAboutNewNewsComments_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowHeaderRssUrl_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowNewsOnMainPage_OverrideForStore, options => options.Ignore());
            CreateMap<NewsSettingsModel, NewsSettings>();
        }

        /// <summary>
        /// Create orders maps 
        /// </summary>
        protected virtual void CreateOrdersMaps()
        {
            CreateMap<Order, UserOrderModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.OrderStatus, options => options.Ignore())
                .ForMember(model => model.PaymentStatus, options => options.Ignore())
                .ForMember(model => model.ShippingStatus, options => options.Ignore())
                .ForMember(model => model.OrderTotal, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());

            CreateMap<OrderNote, OrderNoteModel>()
                .ForMember(model => model.DownloadGuid, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());

            CreateMap<CheckoutAttribute, CheckoutAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.AvailableTaxCategories, options => options.Ignore())
                .ForMember(model => model.CheckoutAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.ConditionAllowed, options => options.Ignore())
                .ForMember(model => model.ConditionModel, options => options.Ignore());
            CreateMap<CheckoutAttributeModel, CheckoutAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore())
                .ForMember(entity => entity.ConditionAttributeXml, options => options.Ignore());

            CreateMap<CheckoutAttributeValue, CheckoutAttributeValueModel>()
                .ForMember(model => model.BaseWeightIn, options => options.Ignore())
                .ForMember(model => model.DisplayColorSquaresRgb, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<CheckoutAttributeValueModel, CheckoutAttributeValue>();

            CreateMap<GiftCard, GiftCardModel>()
                .ForMember(model => model.AmountStr, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.GiftCardUsageHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.PurchasedWithOrderId, options => options.Ignore())
                .ForMember(model => model.PurchasedWithOrderNumber, options => options.Ignore())
                .ForMember(model => model.RemainingAmountStr, options => options.Ignore());
            CreateMap<GiftCardModel, GiftCard>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.GiftCardType, options => options.Ignore())
                .ForMember(entity => entity.IsRecipientNotified, options => options.Ignore())
                .ForMember(entity => entity.PurchasedWithOrderItemId, options => options.Ignore());

            CreateMap<GiftCardUsageHistory, GiftCardUsageHistoryModel>()
                .ForMember(model => model.OrderId, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumber, options => options.Ignore())
                .ForMember(entity => entity.CreatedOn, options => options.Ignore())
                .ForMember(entity => entity.UsedValue, options => options.Ignore());

            CreateMap<OrderSettings, OrderSettingsModel>()
                .ForMember(model => model.AllowAdminsToBuyCallForPriceTvChannels_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowTvChannelThumbnailInOrderDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AnonymousCheckoutAllowed_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderProcessingEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CheckoutDisabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumberMask_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DeleteGiftCardUsageHistory_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisableBillingAddressCheckoutStep_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisableOrderCompletedPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ExportWithTvChannels_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IsReOrderAllowed_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinOrderSubtotalAmountIncludingTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinOrderSubtotalAmount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MinOrderTotalAmount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfDaysReturnRequestAvailable_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OnePageCheckoutEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OrderIdent, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.ReturnRequestActionSearchModel, options => options.Ignore())
                .ForMember(model => model.ReturnRequestNumberMask_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReturnRequestReasonSearchModel, options => options.Ignore())
                .ForMember(model => model.ReturnRequestsAllowFiles_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReturnRequestsEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TermsOfServiceOnOrderConfirmPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TermsOfServiceOnShoppingCartPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<OrderSettingsModel, OrderSettings>()
                .ForMember(settings => settings.GeneratePdfInvoiceInUserLanguage, options => options.Ignore())
                .ForMember(settings => settings.MinimumOrderPlacementInterval, options => options.Ignore())
                .ForMember(settings => settings.DisplayUserCurrencyOnOrders, options => options.Ignore())
                .ForMember(settings => settings.ReturnRequestsFileMaximumSize, options => options.Ignore())
                .ForMember(settings => settings.DisplayOrderSummary, options => options.Ignore());

            CreateMap<ReturnRequestAction, ReturnRequestActionModel>();
            CreateMap<ReturnRequestActionModel, ReturnRequestAction>();

            CreateMap<ReturnRequestReason, ReturnRequestReasonModel>();
            CreateMap<ReturnRequestReasonModel, ReturnRequestReason>();

            CreateMap<ReturnRequest, ReturnRequestModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.UserInfo, options => options.Ignore())
                .ForMember(model => model.ReturnRequestStatusStr, options => options.Ignore())
                .ForMember(model => model.TvChannelId, options => options.Ignore())
                .ForMember(model => model.TvChannelName, options => options.Ignore())
                .ForMember(model => model.OrderId, options => options.Ignore())
                .ForMember(model => model.AttributeInfo, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumber, options => options.Ignore())
                .ForMember(model => model.UploadedFileGuid, options => options.Ignore())
                .ForMember(model => model.ReturnRequestStatusStr, options => options.Ignore());
            CreateMap<ReturnRequestModel, ReturnRequest>()
                 .ForMember(entity => entity.CustomNumber, options => options.Ignore())
                 .ForMember(entity => entity.StoreId, options => options.Ignore())
                 .ForMember(entity => entity.OrderItemId, options => options.Ignore())
                 .ForMember(entity => entity.UploadedFileId, options => options.Ignore())
                 .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                 .ForMember(entity => entity.ReturnRequestStatus, options => options.Ignore())
                 .ForMember(entity => entity.UserId, options => options.Ignore())
                 .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

            CreateMap<ShoppingCartSettings, ShoppingCartSettingsModel>()
                .ForMember(model => model.AllowAnonymousUsersToEmailWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowCartItemEditing_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CartsSharedBetweenStores_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CrossSellsNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayCartAfterAddingTvChannel_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayWishlistAfterAddingTvChannel_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EmailWishlistEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumShoppingCartItems_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumWishlistItems_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MiniShoppingCartEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MiniShoppingCartTvChannelNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MoveItemsFromWishlistToCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowDiscountBox_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowGiftCardBox_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowTvChannelImagesInMiniShoppingCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowTvChannelImagesOnShoppingCart_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowTvChannelImagesOnWishList_OverrideForStore, options => options.Ignore());
            CreateMap<ShoppingCartSettingsModel, ShoppingCartSettings>()
                .ForMember(settings => settings.RenderAssociatedAttributeValueQuantity, options => options.Ignore())
                .ForMember(settings => settings.RoundPricesDuringCalculation, options => options.Ignore());

            CreateMap<ShoppingCartItem, ShoppingCartItemModel>()
                .ForMember(model => model.Store, options => options.Ignore())
                .ForMember(model => model.AttributeInfo, options => options.Ignore())
                .ForMember(model => model.UnitPrice, options => options.Ignore())
                .ForMember(model => model.UnitPriceValue, options => options.Ignore())
                .ForMember(model => model.UpdatedOn, options => options.Ignore())
                .ForMember(model => model.TvChannelName, options => options.Ignore())
                .ForMember(model => model.Total, options => options.Ignore())
                .ForMember(model => model.TotalValue, options => options.Ignore());
        }

        /// <summary>
        /// Create payments maps 
        /// </summary>
        protected virtual void CreatePaymentsMaps()
        {
            CreateMap<IPaymentMethod, PaymentMethodModel>()
                .ForMember(model => model.RecurringPaymentType, options => options.Ignore());

            CreateMap<RecurringPayment, RecurringPaymentModel>()
                .ForMember(model => model.UserId, options => options.Ignore())
                .ForMember(model => model.InitialOrderId, options => options.Ignore())
                .ForMember(model => model.NextPaymentDate, options => options.Ignore())
                .ForMember(model => model.StartDate, options => options.Ignore())
                .ForMember(model => model.CyclePeriodStr, options => options.Ignore())
                .ForMember(model => model.PaymentType, options => options.Ignore())
                .ForMember(model => model.CanCancelRecurringPayment, options => options.Ignore())
                .ForMember(model => model.UserEmail, options => options.Ignore())
                .ForMember(model => model.RecurringPaymentHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.CyclesRemaining, options => options.Ignore());

            CreateMap<RecurringPaymentModel, RecurringPayment>()
                .ForMember(entity => entity.StartDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CyclePeriod, options => options.Ignore())
                .ForMember(entity => entity.InitialOrderId, options => options.Ignore());

            CreateMap<RecurringPaymentHistory, RecurringPaymentHistoryModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.OrderStatus, options => options.Ignore())
                .ForMember(model => model.PaymentStatus, options => options.Ignore())
                .ForMember(model => model.ShippingStatus, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumber, options => options.Ignore());
        }

        /// <summary>
        /// Create plugins maps 
        /// </summary>
        protected virtual void CreatePluginsMaps()
        {
            CreateMap<PluginDescriptor, PluginModel>()
                .ForMember(model => model.CanChangeEnabled, options => options.Ignore())
                .ForMember(model => model.IsEnabled, options => options.Ignore());
        }

        /// <summary>
        /// Create polls maps 
        /// </summary>
        protected virtual void CreatePollsMaps()
        {
            CreateMap<PollAnswer, PollAnswerModel>();
            CreateMap<PollAnswerModel, PollAnswer>();

            CreateMap<Poll, PollModel>()
                .ForMember(model => model.AvailableLanguages, options => options.Ignore())
                .ForMember(model => model.PollAnswerSearchModel, options => options.Ignore())
                .ForMember(model => model.LanguageName, options => options.Ignore());
            CreateMap<PollModel, Poll>();
        }

        /// <summary>
        /// Create security maps 
        /// </summary>
        protected virtual void CreateSecurityMaps()
        {
            CreateMap<CaptchaSettings, CaptchaSettingsModel>()
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReCaptchaPrivateKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReCaptchaPublicKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnApplyVendorPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnBlogCommentPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnContactUsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnEmailTvChannelToFriendPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnEmailWishlistToFriendPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnLoginPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnNewsCommentPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnTvChannelReviewPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnRegistrationPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnForgotPasswordPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnForum_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnCheckoutPageForGuests_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CaptchaType_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReCaptchaV3ScoreThreshold_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CaptchaTypeValues, options => options.Ignore());
            CreateMap<CaptchaSettingsModel, CaptchaSettings>()
                .ForMember(settings => settings.AutomaticallyChooseLanguage, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaDefaultLanguage, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaRequestTimeout, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaTheme, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaApiUrl, options => options.Ignore());
        }

        /// <summary>
        /// Create SEO maps 
        /// </summary>
        protected virtual void CreateSeoMaps()
        {
            CreateMap<UrlRecord, UrlRecordModel>()
                .ForMember(model => model.DetailsUrl, options => options.Ignore())
                .ForMember(model => model.Language, options => options.Ignore())
                .ForMember(model => model.Name, options => options.Ignore());
            CreateMap<UrlRecordModel, UrlRecord>()
                .ForMember(entity => entity.LanguageId, options => options.Ignore())
                .ForMember(entity => entity.Slug, options => options.Ignore());
        }

        /// <summary>
        /// Create shipping maps 
        /// </summary>
        protected virtual void CreateShippingMaps()
        {
            CreateMap<DeliveryDate, DeliveryDateModel>();
            CreateMap<DeliveryDateModel, DeliveryDate>();

            CreateMap<IPickupPointProvider, PickupPointProviderModel>();

            CreateMap<TvChannelAvailabilityRange, TvChannelAvailabilityRangeModel>();
            CreateMap<TvChannelAvailabilityRangeModel, TvChannelAvailabilityRange>();

            CreateMap<ShippingMethod, ShippingMethodModel>();
            CreateMap<ShippingMethodModel, ShippingMethod>();

            CreateMap<IShippingRateComputationMethod, ShippingProviderModel>();

            CreateMap<Shipment, ShipmentModel>()
                .ForMember(model => model.ShippedDate, options => options.Ignore())
                .ForMember(model => model.ReadyForPickupDate, options => options.Ignore())
                .ForMember(model => model.DeliveryDate, options => options.Ignore())
                .ForMember(model => model.TotalWeight, options => options.Ignore())
                .ForMember(model => model.TrackingNumberUrl, options => options.Ignore())
                .ForMember(model => model.Items, options => options.Ignore())
                .ForMember(model => model.ShipmentStatusEvents, options => options.Ignore())
                .ForMember(model => model.PickupInStore, options => options.Ignore())
                .ForMember(model => model.CanShip, options => options.Ignore())
                .ForMember(model => model.CanMarkAsReadyForPickup, options => options.Ignore())
                .ForMember(model => model.CanDeliver, options => options.Ignore())
                .ForMember(model => model.CustomOrderNumber, options => options.Ignore());

            CreateMap<ShippingSettings, ShippingSettingsModel>()
                .ForMember(model => model.AllowPickupInStore_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ConsiderAssociatedTvChannelsDimensions_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SortShippingValues, options => options.Ignore())
                .ForMember(model => model.ShippingSorting_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayPickupPointsOnMap_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayShipmentEventsToUsers_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayShipmentEventsToStoreOwner_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EstimateShippingCartPageEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EstimateShippingTvChannelPageEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EstimateShippingCityNameEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FreeShippingOverXEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FreeShippingOverXIncludingTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FreeShippingOverXValue_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.GoogleMapsApiKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.HideShippingTotal_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyUserAboutShippingFromMultipleLocations_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.ShippingOriginAddress, options => options.Ignore())
                .ForMember(model => model.ShippingOriginAddress_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShipToSameAddress_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.UseWarehouseLocation_OverrideForStore, options => options.Ignore());
            CreateMap<ShippingSettingsModel, ShippingSettings>()
                .ForMember(settings => settings.ActivePickupPointProviderSystemNames, options => options.Ignore())
                .ForMember(settings => settings.ActiveShippingRateComputationMethodSystemNames, options => options.Ignore())
                .ForMember(settings => settings.ReturnValidOptionsIfThereAreAny, options => options.Ignore())
                .ForMember(settings => settings.ShipSeparatelyOneItemEach, options => options.Ignore())
                .ForMember(settings => settings.UseCubeRootMethod, options => options.Ignore())
                .ForMember(settings => settings.RequestDelay, options => options.Ignore());
        }

        /// <summary>
        /// Create stores maps 
        /// </summary>
        protected virtual void CreateStoresMaps()
        {
            CreateMap<Store, StoreModel>()
                .ForMember(model => model.AvailableLanguages, options => options.Ignore());
            CreateMap<StoreModel, Store>()
                .ForMember(entity => entity.SslEnabled, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore());
        }

        /// <summary>
        /// Create tasks maps 
        /// </summary>
        protected virtual void CreateTasksMaps()
        {
            CreateMap<ScheduleTask, ScheduleTaskModel>();
            CreateMap<ScheduleTaskModel, ScheduleTask>()
                .ForMember(entity => entity.Type, options => options.Ignore())
                .ForMember(entity => entity.LastStartUtc, options => options.Ignore())
                .ForMember(entity => entity.LastEndUtc, options => options.Ignore())
                .ForMember(entity => entity.LastSuccessUtc, options => options.Ignore())
                .ForMember(entity => entity.LastEnabledUtc, options => options.Ignore());
        }

        /// <summary>
        /// Create tax maps 
        /// </summary>
        protected virtual void CreateTaxMaps()
        {
            CreateMap<TaxCategory, TaxCategoryModel>();
            CreateMap<TaxCategoryModel, TaxCategory>();

            CreateMap<ITaxProvider, TaxProviderModel>()
                .ForMember(model => model.IsPrimaryTaxProvider, options => options.Ignore());

            CreateMap<TaxSettings, TaxSettingsModel>()
                .ForMember(model => model.AllowUsersToSelectTaxDisplayType_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultTaxAddress, options => options.Ignore())
                .ForMember(model => model.DefaultTaxAddress_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultTaxCategoryId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxRates_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DisplayTaxSuffix_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatAllowVatExemption_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatAssumeValid_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatEnabledForGuests_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatShopCountries, options => options.Ignore())
                .ForMember(model => model.EuVatShopCountryId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EuVatUseWebService_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.HideTaxInOrderSummary_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.HideZeroTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PricesIncludeTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShippingIsTaxable_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShippingPriceIncludesTax_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShippingTaxClassId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TaxBasedOnPickupPointAddress_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TaxBasedOnValues, options => options.Ignore())
                .ForMember(model => model.TaxBasedOn_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TaxCategories, options => options.Ignore())
                .ForMember(model => model.TaxDisplayTypeValues, options => options.Ignore())
                .ForMember(model => model.TaxDisplayType_OverrideForStore, options => options.Ignore());
            CreateMap<TaxSettingsModel, TaxSettings>()
                .ForMember(settings => settings.ActiveTaxProviderSystemName, options => options.Ignore())
                .ForMember(settings => settings.LogErrors, options => options.Ignore());
        }

        /// <summary>
        /// Create topics maps 
        /// </summary>
        protected virtual void CreateTopicsMaps()
        {
            CreateMap<Topic, TopicModel>()
                .ForMember(model => model.AvailableTopicTemplates, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.TopicName, options => options.Ignore())
                .ForMember(model => model.Url, options => options.Ignore());
            CreateMap<TopicModel, Topic>();

            CreateMap<TopicTemplate, TopicTemplateModel>();
            CreateMap<TopicTemplateModel, TopicTemplate>();
        }

        /// <summary>
        /// Create vendors maps 
        /// </summary>
        protected virtual void CreateVendorsMaps()
        {
            CreateMap<Vendor, VendorModel>()
                .ForMember(model => model.Address, options => options.Ignore())
                .ForMember(model => model.AddVendorNoteMessage, options => options.Ignore())
                .ForMember(model => model.AssociatedUsers, options => options.Ignore())
                .ForMember(model => model.SeName, options => options.Ignore())
                .ForMember(model => model.VendorAttributes, options => options.Ignore())
                .ForMember(model => model.VendorNoteSearchModel, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore());
            CreateMap<VendorModel, Vendor>()
                .ForMember(entity => entity.Deleted, options => options.Ignore());

            CreateMap<VendorNote, VendorNoteModel>()
               .ForMember(model => model.CreatedOn, options => options.Ignore())
               .ForMember(model => model.Note, options => options.Ignore());

            CreateMap<VendorAttribute, VendorAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.VendorAttributeValueSearchModel, options => options.Ignore());
            CreateMap<VendorAttributeModel, VendorAttribute>()
                .ForMember(entity => entity.AttributeControlType, options => options.Ignore());

            CreateMap<VendorAttributeValue, VendorAttributeValueModel>();
            CreateMap<VendorAttributeValueModel, VendorAttributeValue>();

            CreateMap<VendorSettings, VendorSettingsModel>()
                .ForMember(model => model.AllowUsersToApplyForVendorAccount_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowUsersToContactVendors_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowSearchByVendor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowVendorsToEditInfo_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AllowVendorsToImportTvChannels_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumTvChannelNumber_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowVendorOnOrderDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowVendorOnTvChannelDetailsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TermsOfServiceEnabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.VendorAttributeSearchModel, options => options.Ignore())
                .ForMember(model => model.VendorsBlockItemsToDisplay_OverrideForStore, options => options.Ignore());
            CreateMap<VendorSettingsModel, VendorSettings>()
                .ForMember(settings => settings.DefaultVendorPageSizeOptions, options => options.Ignore());
        }

        /// <summary>
        /// Create warehouse maps 
        /// </summary>
        protected virtual void CreateWarehouseMaps()
        {
            CreateMap<Warehouse, WarehouseModel>()
                .ForMember(entity => entity.Address, options => options.Ignore());
            CreateMap<WarehouseModel, Warehouse>()
                .ForMember(entity => entity.AddressId, options => options.Ignore());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

        #endregion
    }
}