using TVProgViewer.Core.Domain.Topics;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Topics
{
    /// <summary>
    /// Represents a topic template cache event consumer
    /// </summary>
    public partial class TopicTemplateCacheEventConsumer : CacheEventConsumer<TopicTemplate>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(TopicTemplate entity, EntityEventType entityEventType)
        {
            Remove(TvProgTopicCachingDefaults.TopicTemplatesAllCacheKey);
        }

    }
}
