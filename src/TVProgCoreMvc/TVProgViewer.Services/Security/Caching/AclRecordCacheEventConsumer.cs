using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Security.Caching
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
        protected override async Task ClearCacheAsync(AclRecord entity)
        {
            await RemoveAsync(TvProgSecurityDefaults.AclRecordCacheKey, entity.EntityId, entity.EntityName);
            await RemoveByPrefixAsync(TvProgSecurityDefaults.EntityAclRecordExistsPrefix, entity.EntityName);
        }
    }
}
