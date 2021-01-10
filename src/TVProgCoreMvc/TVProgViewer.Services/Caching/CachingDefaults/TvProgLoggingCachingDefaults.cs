using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to logging services
    /// </summary>
    public static partial class TvProgLoggingCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ActivityTypeAllCacheKey => new CacheKey("TVProgViewer.activitytype.all");
    }
}