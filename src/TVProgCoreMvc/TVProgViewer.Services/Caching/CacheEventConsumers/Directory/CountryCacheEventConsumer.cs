using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Directory
{
    /// <summary>
    /// Represents a country cache event consumer
    /// </summary>
    public partial class CountryCacheEventConsumer : CacheEventConsumer<Country>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Country entity)
        {
            RemoveByPrefix(TvProgDirectoryCachingDefaults.CountriesPrefixCacheKey);
        }
    }
}