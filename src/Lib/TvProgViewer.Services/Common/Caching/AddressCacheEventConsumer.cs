using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Services.Caching;
using TvProgViewer.Services.Users;

namespace TvProgViewer.Services.Common.Caching
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(Address entity)
        {
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserAddressesPrefix);
        }
    }
}
