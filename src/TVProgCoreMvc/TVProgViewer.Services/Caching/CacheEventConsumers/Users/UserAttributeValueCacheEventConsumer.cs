using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Users
{
    /// <summary>
    /// Represents a User attribute value cache event consumer
    /// </summary>
    public partial class UserAttributeValueCacheEventConsumer : CacheEventConsumer<UserAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(UserAttributeValue entity)
        {
            Remove(TvProgUserServiceCachingDefaults.UserAttributesAllCacheKey);
            Remove(TvProgUserServiceCachingDefaults.UserAttributeValuesAllCacheKey.FillCacheKey(entity));
        }
    }
}