using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Users
{
    /// <summary>
    /// Represents a User address mapping cache event consumer
    /// </summary>
    public partial class UserAddressMappingCacheEventConsumer : CacheEventConsumer<UserAddressMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(UserAddressMapping entity)
        {
            RemoveByPrefix(TvProgUserServiceCachingDefaults.UserAddressesPrefixCacheKey, false);
        }
    }
}