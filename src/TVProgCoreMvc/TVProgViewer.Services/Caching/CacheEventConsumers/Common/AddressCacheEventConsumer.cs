using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Common
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
        protected override void ClearCache(Address entity)
        {
            RemoveByPrefix(TvProgUserServiceCachingDefaults.UserAddressesPrefixCacheKey, false);
        }
    }
}
