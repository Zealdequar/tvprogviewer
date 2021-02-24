﻿using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
﻿using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;
using TVProgViewer.Services.Events;

namespace TVProgViewer.Services.Users.Caching
{
    /// <summary>
    /// Represents a user cache event consumer
    /// </summary>
    public partial class UserCacheEventConsumer : CacheEventConsumer<User>, IConsumer<UserPasswordChangedEvent>
    {
        #region Methods

        /// <summary>
        /// Handle password changed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public async Task HandleEventAsync(UserPasswordChangedEvent eventMessage)
        {
            await RemoveAsync(TvProgUserServicesDefaults.UserPasswordLifetimeCacheKey, eventMessage.Password.UserId);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(User entity)
        {
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserUserRolesPrefix);
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserAddressesPrefix);
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<ShoppingCartItem>.AllPrefix);

            if (string.IsNullOrEmpty(entity.SystemName))
                return;

            await RemoveAsync(TvProgUserServicesDefaults.UserBySystemNameCacheKey, entity.SystemName);
            await RemoveAsync(TvProgUserServicesDefaults.UserByGuidCacheKey, entity.UserGuid);
        }

        #endregion
    }
}