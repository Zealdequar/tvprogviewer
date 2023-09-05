using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum cache event consumer
    /// </summary>
    public partial class ForumCacheEventConsumer : CacheEventConsumer<Forum>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Forum entity)
        {
            await RemoveAsync(TvProgForumDefaults.ForumByForumGroupCacheKey, entity.ForumGroupId);
        }
    }
}
