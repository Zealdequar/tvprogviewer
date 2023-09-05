using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Users.Caching
{
    /// <summary>
    /// Represents a user address mapping cache event consumer
    /// </summary>
    public partial class UserAddressMappingCacheEventConsumer : CacheEventConsumer<UserAddressMapping>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(UserAddressMapping entity, EntityEventType entityEventType)
        {
            await RemoveAsync(TvProgUserServicesDefaults.UserAddressesCacheKey, entity.UserId);

            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(TvProgUserServicesDefaults.UserAddressCacheKey, entity.UserId, entity.AddressId);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}