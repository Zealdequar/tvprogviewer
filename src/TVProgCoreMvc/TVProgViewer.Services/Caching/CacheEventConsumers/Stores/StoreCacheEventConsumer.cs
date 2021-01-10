using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Stores
{
    /// <summary>
    /// Represents a store cache event consumer
    /// </summary>
    public partial class StoreCacheEventConsumer : CacheEventConsumer<Store>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Store entity)
        {
            Remove(TvProgStoreCachingDefaults.StoresAllCacheKey);
            RemoveByPrefix(TvProgOrderCachingDefaults.ShoppingCartPrefixCacheKey, false);

            var prefix = TvProgLocalizationCachingDefaults.LanguagesByStoreIdPrefixCacheKey.ToCacheKey(entity);

            RemoveByPrefix(prefix);

        }
    }
}
