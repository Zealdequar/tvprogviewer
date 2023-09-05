using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Security.Caching
{
    /// <summary>
    /// Represents a permission record-user role mapping cache event consumer
    /// </summary>
    public partial class PermissionRecordUserRoleMappingCacheEventConsumer : CacheEventConsumer<PermissionRecordUserRoleMapping>
    {
    }
}