using TVProgViewer.Core.Domain.Topics;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Topics
{
    /// <summary>
    /// Represents a topic cache event consumer
    /// </summary>
    public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Topic entity)
        {
            RemoveByPrefix(TvProgTopicCachingDefaults.TopicsAllPrefixCacheKey);
            var prefix = TvProgTopicCachingDefaults.TopicBySystemNamePrefixCacheKey.ToCacheKey(entity.SystemName);
            RemoveByPrefix(prefix);

        }
    }
}
