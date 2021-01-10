using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Common
{
    /// <summary>
    /// Represents a address attribute value cache event consumer
    /// </summary>
    public partial class AddressAttributeValueCacheEventConsumer : CacheEventConsumer<AddressAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(AddressAttributeValue entity)
        {
            Remove(TvProgCommonCachingDefaults.AddressAttributesAllCacheKey);
            var cacheKey = TvProgCommonCachingDefaults.AddressAttributeValuesAllCacheKey.FillCacheKey(entity.AddressAttributeId);
            Remove(cacheKey);
        }
    }
}
