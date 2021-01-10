using TVProgViewer.Core.Domain.News;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.News
{
    /// <summary>
    /// Represents a news item cache event consumer
    /// </summary>
    public partial class NewsItemCacheEventConsumer : CacheEventConsumer<NewsItem>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(NewsItem entity)
        {
            var prefix = TvProgNewsCachingDefaults.NewsCommentsNumberPrefixCacheKey.ToCacheKey(entity);

            RemoveByPrefix(prefix);
        }
    }
}