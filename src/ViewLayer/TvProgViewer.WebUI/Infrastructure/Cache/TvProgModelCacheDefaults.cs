using TvProgViewer.Core.Caching;

namespace TvProgViewer.WebUI.Infrastructure.Cache
{
    public static partial class TvProgModelCacheDefaults
    {
        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerNavigationModelKey => new("TvProg.pres.manufacturer.navigation-{0}-{1}-{2}-{3}", ManufacturerNavigationPrefixCacheKey);
        public static string ManufacturerNavigationPrefixCacheKey => "TvProg.pres.manufacturer.navigation";

        /// <summary>
        /// Key for VendorNavigationModel caching
        /// </summary>
        public static CacheKey VendorNavigationModelKey => new("TvProg.pres.vendor.navigation");

        /// <summary>
        /// Key for list of CategorySimpleModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CategoryAllModelKey => new("TvProg.pres.category.all-{0}-{1}-{2}", CategoryAllPrefixCacheKey);
        public static string CategoryAllPrefixCacheKey => "TvProg.pres.category.all";

        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// {2} : picture size
        /// {3} : language ID
        /// {4} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public static CacheKey CategoryHomepageKey => new("TvProg.pres.category.homepage-{0}-{1}-{2}-{3}-{4}", CategoryHomepagePrefixCacheKey);
        public static string CategoryHomepagePrefixCacheKey => "TvProg.pres.category.homepage";

        /// <summary>
        /// Key for Xml document of CategorySimpleModels caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CategoryXmlAllModelKey => new("TvProg.pres.categoryXml.all-{0}-{1}-{2}", CategoryXmlAllPrefixCacheKey);
        public static string CategoryXmlAllPrefixCacheKey => "TvProg.pres.categoryXml.all";

        /// <summary>
        /// Key for bestsellers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public static CacheKey HomepageBestsellersIdsKey => new("TvProg.pres.bestsellers.homepage-{0}", HomepageBestsellersIdsPrefixCacheKey);
        public static string HomepageBestsellersIdsPrefixCacheKey => "TvProg.pres.bestsellers.homepage";

        /// <summary>
        /// Key for "also purchased" tvChannel identifiers displayed on the tvChannel details page
        /// </summary>
        /// <remarks>
        /// {0} : current tvChannel id
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey TvChannelsAlsoPurchasedIdsKey => new("TvProg.pres.alsopuchased-{0}-{1}", TvChannelsAlsoPurchasedIdsPrefixCacheKey);
        public static string TvChannelsAlsoPurchasedIdsPrefixCacheKey => "TvProg.pres.alsopuchased";

        /// <summary>
        /// Key for tvChannel picture caching on the tvChannel catalog pages (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : value indicating whether to display all tvChannel pictures
        /// {4} : language ID ("alt" and "title" can depend on localized tvChannel name)
        /// {5} : is connection SSL secured?
        /// {6} : current store ID
        /// </remarks>
        public static CacheKey TvChannelOverviewPicturesModelKey => new("TvProg.pres.tvChannel.overviewpictures-{0}-{1}-{2}-{3}-{4}-{5}-{6}", TvChannelOverviewPicturesPrefixCacheKey, TvChannelOverviewPicturesPrefixCacheKeyById);
        public static string TvChannelOverviewPicturesPrefixCacheKey => "TvProg.pres.tvChannel.overviewpictures";
        public static string TvChannelOverviewPicturesPrefixCacheKeyById => "TvProg.pres.tvChannel.overviewpictures-{0}-";

        /// <summary>
        /// Key for tvChannel picture caching on the tvChannel details page (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel id
        /// {1} : picture size
        /// {2} : isAssociatedTvChannel?
        /// {3} : language ID ("alt" and "title" can depend on localized tvChannel name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey TvChannelDetailsPicturesModelKey => new("TvProg.pres.tvChannel.detailspictures-{0}-{1}-{2}-{3}-{4}-{5}", TvChannelDetailsPicturesPrefixCacheKey, TvChannelDetailsPicturesPrefixCacheKeyById);
        public static string TvChannelDetailsPicturesPrefixCacheKey => "TvProg.pres.tvChannel.detailspictures";
        public static string TvChannelDetailsPicturesPrefixCacheKeyById => "TvProg.pres.tvChannel.detailspictures-{0}-";

        /// <summary>
        /// Key for tvChannel reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel id
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey TvChannelReviewsModelKey => new("TvProg.pres.tvChannel.reviews-{0}-{1}", TvChannelReviewsPrefixCacheKey, TvChannelReviewsPrefixCacheKeyById);

        public static string TvChannelReviewsPrefixCacheKey => "TvProg.pres.tvChannel.reviews";
        public static string TvChannelReviewsPrefixCacheKeyById => "TvProg.pres.tvChannel.reviews-{0}-";

        /// <summary>
        /// Key for tvChannel attribute picture caching on the tvChannel details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey TvChannelAttributePictureModelKey => new("TvProg.pres.tvchannelattribute.picture-{0}-{1}-{2}", TvChannelAttributePicturePrefixCacheKey);
        public static string TvChannelAttributePicturePrefixCacheKey => "TvProg.pres.tvchannelattribute.picture";

        /// <summary>
        /// Key for tvChannel attribute picture caching on the tvChannel details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey TvChannelAttributeImageSquarePictureModelKey => new("TvProg.pres.tvchannelattribute.imagesquare.picture-{0}-{1}-{2}", TvChannelAttributeImageSquarePicturePrefixCacheKey);
        public static string TvChannelAttributeImageSquarePicturePrefixCacheKey => "TvProg.pres.tvchannelattribute.imagesquare.picture";

        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey CategoryPictureModelKey => new("TvProg.pres.category.picture-{0}-{1}-{2}-{3}-{4}-{5}", CategoryPicturePrefixCacheKey, CategoryPicturePrefixCacheKeyById);
        public static string CategoryPicturePrefixCacheKey => "TvProg.pres.category.picture";
        public static string CategoryPicturePrefixCacheKeyById => "TvProg.pres.category.picture-{0}-";

        /// <summary>
        /// Key for manufacturer picture caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized manufacturer name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerPictureModelKey => new("TvProg.pres.manufacturer.picture-{0}-{1}-{2}-{3}-{4}-{5}", ManufacturerPicturePrefixCacheKey, ManufacturerPicturePrefixCacheKeyById);
        public static string ManufacturerPicturePrefixCacheKey => "TvProg.pres.manufacturer.picture";
        public static string ManufacturerPicturePrefixCacheKeyById => "TvProg.pres.manufacturer.picture-{0}-";

        /// <summary>
        /// Key for vendor picture caching
        /// </summary>
        /// <remarks>
        /// {0} : vendor id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey VendorPictureModelKey => new("TvProg.pres.vendor.picture-{0}-{1}-{2}-{3}-{4}-{5}", VendorPicturePrefixCacheKey, VendorPicturePrefixCacheKeyById);
        public static string VendorPicturePrefixCacheKey => "TvProg.pres.vendor.picture";
        public static string VendorPicturePrefixCacheKeyById => "TvProg.pres.vendor.picture-{0}-";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : shopping cart item id
        /// P.S. we could cache by tvChannel ID. it could increase performance.
        /// but it won't work for tvChannel attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized tvChannel name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey CartPictureModelKey => new("TvProg.pres.cart.picture-{0}-{1}-{2}-{3}-{4}-{5}", CartPicturePrefixCacheKey);
        public static string CartPicturePrefixCacheKey => "TvProg.pres.cart.picture";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : order item id
        /// P.S. we could cache by tvChannel ID. it could increase performance.
        /// but it won't work for tvChannel attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized tvChannel name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey OrderPictureModelKey => new("TvProg.pres.order.picture-{0}-{1}-{2}-{3}-{4}-{5}", OrderPicturePrefixCacheKey);
        public static string OrderPicturePrefixCacheKey => "TvProg.pres.order.picture";

        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey HomepagePollsModelKey => new("TvProg.pres.poll.homepage-{0}-{1}", PollsPrefixCacheKey);
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey PollBySystemNameModelKey => new("TvProg.pres.poll.systemname-{0}-{1}-{2}", PollsPrefixCacheKey);
        public static string PollsPrefixCacheKey => "TvProg.pres.poll";

        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey BlogMonthsModelKey => new("TvProg.pres.blog.months-{0}-{1}", BlogPrefixCacheKey);
        public static string BlogPrefixCacheKey => "TvProg.pres.blog";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey HomepageNewsModelKey => new("TvProg.pres.news.homepage-{0}-{1}", NewsPrefixCacheKey);
        public static string NewsPrefixCacheKey => "TvProg.pres.news";

        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : current theme
        /// {2} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public static CacheKey StoreLogoPath => new("TvProg.pres.logo-{0}-{1}-{2}", StoreLogoPathPrefixCacheKey);
        public static string StoreLogoPathPrefixCacheKey => "TvProg.pres.logo";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey SitemapPageModelKey => new("TvProg.pres.sitemap.page-{0}-{1}-{2}", SitemapPrefixCacheKey);
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : sitemap identifier
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static CacheKey SitemapSeoModelKey => new("TvProg.pres.sitemap.seo-{0}-{1}-{2}-{3}", SitemapPrefixCacheKey);
        public static string SitemapPrefixCacheKey => "TvProg.pres.sitemap";

        /// <summary>
        /// Key for widget info
        /// </summary>
        /// <remarks>
        /// {0} : current user role IDs hash
        /// {1} : current store ID
        /// {2} : widget zone
        /// {3} : current theme name
        /// </remarks>
        public static CacheKey WidgetModelKey => new("TvProg.pres.widget-{0}-{1}-{2}-{3}", WidgetPrefixCacheKey);
        public static string WidgetPrefixCacheKey => "TvProg.pres.widget";

    }
}
