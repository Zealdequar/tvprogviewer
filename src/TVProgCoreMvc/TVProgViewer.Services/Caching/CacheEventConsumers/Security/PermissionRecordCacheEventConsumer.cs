using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Security
{
    /// <summary>
    /// Represents a permission record cache event consumer
    /// </summary>
    public partial class PermissionRecordCacheEventConsumer : CacheEventConsumer<PermissionRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(PermissionRecord entity)
        {
            var prefix = TvProgSecurityCachingDefaults.PermissionsAllowedPrefixCacheKey.ToCacheKey(entity.SystemName);
            RemoveByPrefix(prefix);
            RemoveByPrefix(TvProgSecurityCachingDefaults.PermissionsAllByUserRoleIdPrefixCacheKey);
        }
    }
}
