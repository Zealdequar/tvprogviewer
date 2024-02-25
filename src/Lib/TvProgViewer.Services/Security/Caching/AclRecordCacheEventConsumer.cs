using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Security.Caching
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(AclRecord entity)
        {
            await RemoveAsync(TvProgSecurityDefaults.AclRecordCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(TvProgSecurityDefaults.EntityAclRecordExistsCacheKey, entity.EntityName);
        }
    }
}
