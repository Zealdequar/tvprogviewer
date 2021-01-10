using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching.CachingDefaults;
using TVProgViewer.Services.Events;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Users
{
    /// <summary>
    /// Represents a User cache event consumer
    /// </summary>
    public partial class UserCacheEventConsumer : CacheEventConsumer<User>, IConsumer<UserPasswordChangedEvent>
    {
        #region Methods

        /// <summary>
        /// Handle password changed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(UserPasswordChangedEvent eventMessage)
        {
            Remove(TvProgUserServiceCachingDefaults.UserPasswordLifetimeCacheKey.FillCacheKey(eventMessage.Password.UserId));
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(User entity)
        {
            RemoveByPrefix(TvProgUserServiceCachingDefaults.UserUserRolesPrefixCacheKey, false);
            RemoveByPrefix(TvProgUserServiceCachingDefaults.UserAddressesPrefixCacheKey, false);
            RemoveByPrefix(TvProgOrderCachingDefaults.ShoppingCartPrefixCacheKey, false);
        }

        #endregion
    }
}