using TVProgViewer.Core.Domain.News;
using TVProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TVProgViewer.Services.News.Caching
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
        /// <param name="entityEventType">Entity event type</param>
        protected override async Task ClearCacheAsync(NewsItem entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
                await RemoveByPrefixAsync(TvProgNewsDefaults.NewsCommentsNumberPrefix, entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}