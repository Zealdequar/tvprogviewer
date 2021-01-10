using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Users
{
    /// <summary>
    /// Represents a User role cache event consumer
    /// </summary>
    public partial class UserRoleCacheEventConsumer : CacheEventConsumer<UserRole>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(UserRole entity)
        {
            RemoveByPrefix(TvProgUserServiceCachingDefaults.UserRolesPrefixCacheKey);
            RemoveByPrefix(TvProgUserServiceCachingDefaults.UserUserRolesPrefixCacheKey, false);
        }
    }
}
