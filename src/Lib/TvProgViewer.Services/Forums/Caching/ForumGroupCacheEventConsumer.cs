using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TvProgViewer.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum group cache event consumer
    /// </summary>
    public partial class ForumGroupCacheEventConsumer : CacheEventConsumer<ForumGroup>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(ForumGroup entity)
        {
            await RemoveAsync(TvProgForumDefaults.ForumByForumGroupCacheKey, entity);
        }
    }
}
