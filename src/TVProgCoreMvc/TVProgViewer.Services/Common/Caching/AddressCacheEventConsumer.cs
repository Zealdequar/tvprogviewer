using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Caching;
using TVProgViewer.Services.Users;

namespace TVProgViewer.Services.Common.Caching
{
    /// <summary>
    /// Represents a address cache event consumer
    /// </summary>
    public partial class AddressCacheEventConsumer : CacheEventConsumer<Address>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Address entity)
        {
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserAddressesPrefix);
        }
    }
}
