using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Security.Caching
{
    /// <summary>
    /// Represents a permission record-user role mapping cache event consumer
    /// </summary>
    public partial class PermissionRecordUserRoleMappingCacheEventConsumer : CacheEventConsumer<PermissionRecordUserRoleMapping>
    {
    }
}