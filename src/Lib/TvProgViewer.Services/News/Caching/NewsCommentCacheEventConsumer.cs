using TvProgViewer.Core.Domain.News;
using TvProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TvProgViewer.Services.News.Caching
{
    /// <summary>
    /// Represents a news comment cache event consumer
    /// </summary>
    public partial class NewsCommentCacheEventConsumer : CacheEventConsumer<NewsComment>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(NewsComment entity)
        {
            await RemoveByPrefixAsync(TvProgNewsDefaults.NewsCommentsNumberPrefix, entity.NewsItemId);
        }
    }
}