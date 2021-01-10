using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Directory
{
    /// <summary>
    /// Represents a currency cache event consumer
    /// </summary>
    public partial class CurrencyCacheEventConsumer : CacheEventConsumer<Currency>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Currency entity)
        {
            RemoveByPrefix(TvProgDirectoryCachingDefaults.CurrenciesAllPrefixCacheKey);
        }
    }
}
