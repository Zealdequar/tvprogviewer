using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Blogs.Caching
{
    /// <summary>
    /// Represents a blog comment cache event consumer
    /// </summary>
    public partial class BlogCommentCacheEventConsumer : CacheEventConsumer<BlogComment>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(BlogComment entity)
        {
            await RemoveByPrefixAsync(TvProgBlogsDefaults.BlogCommentsNumberPrefix, entity.BlogPostId);
        }
    }
}