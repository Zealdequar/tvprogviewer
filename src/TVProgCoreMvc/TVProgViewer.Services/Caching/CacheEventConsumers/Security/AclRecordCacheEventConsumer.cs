using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Security
{
    /// <summary>
    /// Represents a ACL record cache event consumer
    /// </summary>
    public partial class AclRecordCacheEventConsumer : CacheEventConsumer<AclRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(AclRecord entity)
        {
            var cacheKey = TvProgSecurityCachingDefaults.AclRecordByEntityIdNameCacheKey.FillCacheKey(entity.EntityId, entity.EntityName);
            Remove(cacheKey);
        }
    }
}
