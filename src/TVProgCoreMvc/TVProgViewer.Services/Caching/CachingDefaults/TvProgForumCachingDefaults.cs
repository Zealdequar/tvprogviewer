using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to forums services
    /// </summary>
    public static partial class TvProgForumCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ForumGroupAllCacheKey => new CacheKey("TVProgViewer.forumgroup.all");
        
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : forum group ID
        /// </remarks>
        public static CacheKey ForumAllByForumGroupIdCacheKey => new CacheKey("TVProgViewer.forum.allbyforumgroupid-{0}");
    }
}