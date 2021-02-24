using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Blogs.Caching
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
        protected override async Task ClearCacheAsync(BlogComment entity)
        {
            await RemoveByPrefixAsync(TvProgBlogsDefaults.BlogCommentsNumberPrefix, entity.BlogPostId);
        }
    }
}