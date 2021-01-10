using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to tax services
    /// </summary>
    public static partial class TvProgTaxCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey TaxCategoriesAllCacheKey => new CacheKey("TVProgViewer.taxcategory.all");
    }
}