using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Common
{
    /// <summary>
    /// Represents a generic attribute cache event consumer
    /// </summary>
    public partial class GenericAttributeCacheEventConsumer : CacheEventConsumer<GenericAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(GenericAttribute entity)
        {
            var cacheKey = TvProgCommonCachingDefaults.GenericAttributeCacheKey.FillCacheKey(entity.EntityId, entity.KeyGroup);
            Remove(cacheKey);
        }
    }
}
