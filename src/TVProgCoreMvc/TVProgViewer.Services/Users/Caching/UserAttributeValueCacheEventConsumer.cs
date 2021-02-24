using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Users.Caching
{
    /// <summary>
    /// Represents a user attribute value cache event consumer
    /// </summary>
    public partial class UserAttributeValueCacheEventConsumer : CacheEventConsumer<UserAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(UserAttributeValue entity)
        {
            await RemoveAsync(TvProgUserServicesDefaults.UserAttributeValuesByAttributeCacheKey, entity.UserAttributeId);
        }
    }
}