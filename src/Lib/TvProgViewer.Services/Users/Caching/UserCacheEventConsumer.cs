﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Caching;
using TvProgViewer.Services.Events;
using TvProgViewer.Services.Orders;

namespace TvProgViewer.Services.Users.Caching
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(UserPasswordChangedEvent eventMessage)
        {
            await RemoveAsync(TvProgUserServicesDefaults.UserPasswordLifetimeCacheKey, eventMessage.Password.UserId);
        }

        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(User entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
            {
                await RemoveAsync(TvProgUserServicesDefaults.UserAddressesCacheKey, entity);
                await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserAddressesByUserPrefix, entity);
            }

            await base.ClearCacheAsync(entity, entityEventType);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(User entity)
        {
            await RemoveByPrefixAsync(TvProgUserServicesDefaults.UserUserRolesByUserPrefix, entity);
            await RemoveByPrefixAsync(TvProgOrderDefaults.ShoppingCartItemsByUserPrefix, entity);
            await RemoveAsync(TvProgUserServicesDefaults.UserByGuidCacheKey, entity.UserGuid);

            if (string.IsNullOrEmpty(entity.SystemName))
                return;

            await RemoveAsync(TvProgUserServicesDefaults.UserBySystemNameCacheKey, entity.SystemName);
        }

        #endregion
    }
}