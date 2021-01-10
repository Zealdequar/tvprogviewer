using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Vendors
{
    /// <summary>
    /// Represents a vendor attribute cache event consumer
    /// </summary>
    public partial class VendorAttributeCacheEventConsumer : CacheEventConsumer<VendorAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(VendorAttribute entity)
        {
            Remove(TvProgVendorsCachingDefaults.VendorAttributesAllCacheKey);

            var cacheKey = TvProgVendorsCachingDefaults.VendorAttributeValuesAllCacheKey.FillCacheKey(entity);

            Remove(cacheKey);

        }
    }
}
