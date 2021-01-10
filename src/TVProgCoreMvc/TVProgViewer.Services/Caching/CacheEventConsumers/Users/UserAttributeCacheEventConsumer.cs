using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Users
{
    /// <summary>
    /// Represents a User attribute cache event consumer
    /// </summary>
    public partial class UserAttributeCacheEventConsumer : CacheEventConsumer<UserAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(UserAttribute entity)
        {
            Remove(TvProgUserServiceCachingDefaults.UserAttributesAllCacheKey);
            Remove(TvProgUserServiceCachingDefaults.UserAttributeValuesAllCacheKey.FillCacheKey(entity));
        }
    }
}
