using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Blogs.Caching
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(BlogPost entity)
        {
           await RemoveByPrefixAsync(TvProgBlogsDefaults.BlogTagsPrefix);
        }
    }
}