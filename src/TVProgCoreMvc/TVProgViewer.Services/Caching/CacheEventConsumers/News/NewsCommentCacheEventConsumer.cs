using TVProgViewer.Core.Domain.News;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.News
{
    /// <summary>
    /// Represents a news comment cache event consumer
    /// </summary>
    public partial class NewsCommentCacheEventConsumer : CacheEventConsumer<NewsComment>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(NewsComment entity, EntityEventType entityEventType)
        {
            if (entityEventType != EntityEventType.Delete)
                return;

            var prefix = TvProgNewsCachingDefaults.NewsCommentsNumberPrefixCacheKey.ToCacheKey(entity.NewsItemId);

            RemoveByPrefix(prefix);
        }
    }
}