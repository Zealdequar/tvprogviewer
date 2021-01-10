using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to vendor services
    /// </summary>
    public static partial class TvProgVendorsCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching all vendor attributes
        /// </summary>
        public static CacheKey VendorAttributesAllCacheKey => new CacheKey("TvProg.vendorattribute.all");
       
        /// <summary>
        /// Gets a key for caching vendor attribute values of the vendor attribute
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute ID
        /// </remarks>
        public static CacheKey VendorAttributeValuesAllCacheKey => new CacheKey("TvProg.vendorattributevalue.all-{0}");
    }
}