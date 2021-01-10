using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to discounts services
    /// </summary>
    public static partial class TvProgDiscountCachingDefaults
    {
        /// <summary>
        /// Key for discount requirement of a certain discount
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// </remarks>
        public static CacheKey DiscountRequirementModelCacheKey => new CacheKey("TVProgViewer.discounts.requirements-{0}");
        
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : coupon code
        /// {2} : discount name
        /// </remarks>
        public static CacheKey DiscountAllCacheKey => new CacheKey("TVProgViewer.discounts.all-{0}-{1}-{2}", DiscountAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string DiscountAllPrefixCacheKey => "TVProgViewer.discounts.all";

        /// <summary>
        /// Key for category IDs of a discount
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey DiscountCategoryIdsModelCacheKey => new CacheKey("TVProgViewer.discounts.categoryids-{0}-{1}-{2}", DiscountCategoryIdsByDiscountPrefixCacheKey, DiscountCategoryIdsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// </remarks>
        public static string DiscountCategoryIdsByDiscountPrefixCacheKey => "TVProgViewer.discounts.categoryids-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string DiscountCategoryIdsPrefixCacheKey => "TVProgViewer.discounts.categoryids";

        /// <summary>
        /// Key for manufacturer IDs of a discount
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey DiscountManufacturerIdsModelCacheKey => new CacheKey("TVProgViewer.discounts.manufacturerids-{0}-{1}-{2}", DiscountManufacturerIdsByDiscountPrefixCacheKey, DiscountManufacturerIdsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// </remarks>
        public static string DiscountManufacturerIdsByDiscountPrefixCacheKey => "TVProgViewer.discounts.manufacturerids-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string DiscountManufacturerIdsPrefixCacheKey => "TVProgViewer.discounts.manufacturerids";
    }
}