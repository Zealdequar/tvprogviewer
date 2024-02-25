using TvProgViewer.Core.Domain.News;
using TvProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TvProgViewer.Services.News.Caching
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(NewsItem entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
                await RemoveByPrefixAsync(TvProgNewsDefaults.NewsCommentsNumberPrefix, entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}