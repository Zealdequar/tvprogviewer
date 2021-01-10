using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to Gdpr services
    /// </summary>
    public static partial class TvProgGdprCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ConsentsAllCacheKey => new CacheKey("TVProgViewer.consents.all");
    }
}