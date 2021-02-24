using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Forums.Caching
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
        protected override async Task ClearCacheAsync(Forum entity)
        {
            await RemoveAsync(TvProgForumDefaults.ForumByForumGroupCacheKey, entity.ForumGroupId);
        }
    }
}
