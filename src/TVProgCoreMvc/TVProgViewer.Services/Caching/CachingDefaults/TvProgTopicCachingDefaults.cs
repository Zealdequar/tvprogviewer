using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to topic services
    /// </summary>
    public static partial class TvProgTopicCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// </remarks>
        public static CacheKey TopicsAllCacheKey => new CacheKey("TVProgViewer.topics.all-{0}-{1}-{2}", TopicsAllPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : show hidden?
        /// {2} : include in top menu?
        /// {3} : customer role IDs hash
        /// </remarks>
        public static CacheKey TopicsAllWithACLCacheKey => new CacheKey("TVProgViewer.topics.all.acl-{0}-{1}-{2}-{3}", TopicsAllPrefixCacheKey);

        /// <summary>
        /// Gets a pattern to clear cache
        /// </summary>
        public static string TopicsAllPrefixCacheKey => "TVProgViewer.topics.all";
        
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : store id
        /// {2} : customer roles Ids hash
        /// </remarks>
        public static CacheKey TopicBySystemNameCacheKey => new CacheKey("TVProgViewer.topics.systemName-{0}-{1}-{2}", TopicBySystemNamePrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// </remarks>
        public static string TopicBySystemNamePrefixCacheKey => "TVProgViewer.topics.systemName-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey TopicTemplatesAllCacheKey => new CacheKey("TVProgViewer.topictemplates.all");
    }
}