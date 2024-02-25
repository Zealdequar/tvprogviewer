using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Events;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.Events;

namespace TvProgViewer.Services.Caching
{
    /// <summary>
    /// Represents the base entity cache event consumer
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract partial class CacheEventConsumer<TEntity> :
        IConsumer<EntityInsertedEvent<TEntity>>,
        IConsumer<EntityUpdatedEvent<TEntity>>,
        IConsumer<EntityDeletedEvent<TEntity>>
        where TEntity : BaseEntity
    {
        #region Fields

        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        protected CacheEventConsumer()
        {
            _staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ClearCacheAsync(TEntity entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<TEntity>.ByIdsPrefix);
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<TEntity>.AllPrefix);

            if (entityEventType != EntityEventType.Insert)
                await RemoveAsync(TvProgEntityCacheDefaults<TEntity>.ByIdCacheKey, entity);

            await ClearCacheAsync(entity);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual Task ClearCacheAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            await _staticCacheManager.RemoveByPrefixAsync(prefix, prefixParameters);
        }

        /// <summary>
        /// Remove the value with the specified key from the cache
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheKeyParameters">Parameters to create cache key</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            await _staticCacheManager.RemoveAsync(cacheKey, cacheKeyParameters);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task HandleEventAsync(EntityInsertedEvent<TEntity> eventMessage)
        {
            await ClearCacheAsync(eventMessage.Entity, EntityEventType.Insert);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task HandleEventAsync(EntityUpdatedEvent<TEntity> eventMessage)
        {
            await ClearCacheAsync(eventMessage.Entity, EntityEventType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task HandleEventAsync(EntityDeletedEvent<TEntity> eventMessage)
        {
            await ClearCacheAsync(eventMessage.Entity, EntityEventType.Delete);
        }

        #endregion

        #region Nested

        protected enum EntityEventType
        {
            Insert,
            Update,
            Delete
        }

        #endregion
    }
}