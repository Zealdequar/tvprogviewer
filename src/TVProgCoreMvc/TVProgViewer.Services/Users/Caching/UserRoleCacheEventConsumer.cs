using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Users.Caching
{
    /// <summary>
    /// Represents a user role cache event consumer
    /// </summary>
    public partial class UserRoleCacheEventConsumer : CacheEventConsumer<UserRole>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(UserRole entity)
        {
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserRolesBySystemNamePrefix);
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserUserRolesPrefix);
        }
    }
}
