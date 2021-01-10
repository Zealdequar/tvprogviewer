using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Common
{
    /// <summary>
    /// Represents a address attribute cache event consumer
    /// </summary>
    public partial class AddressAttributeCacheEventConsumer : CacheEventConsumer<AddressAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(AddressAttribute entity)
        {
            Remove(TvProgCommonCachingDefaults.AddressAttributesAllCacheKey);

            var cacheKey = TvProgCommonCachingDefaults.AddressAttributeValuesAllCacheKey.FillCacheKey(entity);
            Remove(cacheKey);
        }
    }
}
