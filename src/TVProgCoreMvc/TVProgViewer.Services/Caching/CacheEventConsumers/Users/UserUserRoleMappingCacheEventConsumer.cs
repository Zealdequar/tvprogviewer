using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Users
{
    /// <summary>
    /// Represents a User User role mapping cache event consumer
    /// </summary>
    public partial class UserUserRoleMappingCacheEventConsumer : CacheEventConsumer<UserUserRoleMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(UserUserRoleMapping entity)
        {
            RemoveByPrefix(TvProgUserServiceCachingDefaults.UserUserRolesPrefixCacheKey, false);
        }
    }
}