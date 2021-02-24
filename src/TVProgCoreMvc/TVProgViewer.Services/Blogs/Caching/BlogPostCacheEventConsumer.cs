using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Blogs.Caching
{
    /// <summary>
    /// Represents a blog post cache event consumer
    /// </summary>
    public partial class BlogPostCacheEventConsumer : CacheEventConsumer<BlogPost>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(BlogPost entity)
        {
           await RemoveByPrefixAsync(TvProgBlogsDefaults.BlogTagsPrefix);
        }
    }
}