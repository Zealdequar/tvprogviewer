using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Users.Caching
{
    /// <summary>
    /// Represents a user address mapping cache event consumer
    /// </summary>
    public partial class UserAddressMappingCacheEventConsumer : CacheEventConsumer<UserAddressMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(UserAddressMapping entity)
        {
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserAddressesPrefix);
        }
    }
}