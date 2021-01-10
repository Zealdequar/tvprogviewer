using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Blogs
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
        protected override void ClearCache(BlogComment entity)
        {
            RemoveByPrefix(string.Format(TvProgBlogsCachingDefaults.BlogCommentsNumberPrefixCacheKey, entity.BlogPostId));
        }
    }
}