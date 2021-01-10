using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value cache event consumer
    /// </summary>
    public partial class VendorAttributeValueCacheEventConsumer : CacheEventConsumer<VendorAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(VendorAttributeValue entity)
        {
            var cacheKey = TvProgVendorsCachingDefaults.VendorAttributeValuesAllCacheKey.FillCacheKey(entity.VendorAttributeId);

            Remove(cacheKey);
        }
    }
}
