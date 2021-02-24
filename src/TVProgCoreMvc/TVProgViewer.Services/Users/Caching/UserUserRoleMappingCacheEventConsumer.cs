using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Users.Caching
{
    /// <summary>
    /// Represents a user user role mapping cache event consumer
    /// </summary>
    public partial class UserUserRoleMappingCacheEventConsumer : CacheEventConsumer<UserUserRoleMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(UserUserRoleMapping entity)
        {
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserUserRolesPrefix);
        }
    }
}