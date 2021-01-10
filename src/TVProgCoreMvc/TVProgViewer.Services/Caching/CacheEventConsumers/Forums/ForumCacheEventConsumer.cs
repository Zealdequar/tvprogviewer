using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Forums
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
        protected override void ClearCache(Forum entity)
        {
            var cacheKey = TvProgForumCachingDefaults.ForumAllByForumGroupIdCacheKey.FillCacheKey(entity.ForumGroupId);
            Remove(cacheKey);
        }
    }
}
