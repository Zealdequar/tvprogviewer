using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Stores
{
    /// <summary>
    /// Represents a store mapping cache event consumer
    /// </summary>
    public partial class StoreMappingCacheEventConsumer : CacheEventConsumer<StoreMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(StoreMapping entity)
        {
            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var key = TvProgStoreCachingDefaults.StoreMappingsByEntityIdNameCacheKey.FillCacheKey(entityId, entityName);

            Remove(key);

            key = TvProgStoreCachingDefaults.StoreMappingIdsByEntityIdNameCacheKey.FillCacheKey(entityId, entityName);

            Remove(key);
        }
    }
}
