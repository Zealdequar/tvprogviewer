using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Forums
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
        protected override void ClearCache(ForumGroup entity)
        {
            Remove(TvProgForumCachingDefaults.ForumGroupAllCacheKey);
            var cacheKey = TvProgForumCachingDefaults.ForumAllByForumGroupIdCacheKey.FillCacheKey(entity);
            Remove(cacheKey);
        }
    }
}
