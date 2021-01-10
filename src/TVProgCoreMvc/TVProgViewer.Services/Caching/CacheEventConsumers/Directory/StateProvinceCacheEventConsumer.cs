using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Directory
{
    /// <summary>
    /// Represents a state province cache event consumer
    /// </summary>
    public partial class StateProvinceCacheEventConsumer : CacheEventConsumer<StateProvince>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(StateProvince entity)
        {
            RemoveByPrefix(TvProgDirectoryCachingDefaults.StateProvincesPrefixCacheKey);
        }
    }
}
