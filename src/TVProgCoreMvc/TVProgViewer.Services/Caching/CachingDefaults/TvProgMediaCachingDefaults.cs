using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to media services
    /// </summary>
    public static partial class TvProgMediaCachingDefaults
    {
        /// <summary>
        /// Gets a key to cache whether thumb exists
        /// </summary>
        /// <remarks>
        /// {0} : thumb file name
        /// </remarks>
        public static CacheKey ThumbExistsCacheKey => new CacheKey("TVProgViewer.azure.thumb.exists-{0}", ThumbsExistsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ThumbsExistsPrefixCacheKey => "TVProgViewer.azure.thumb.exists";

        /// <summary>
        /// Gets a key to cache
        /// </summary>
        /// <remarks>
        /// {0} : virtual path
        /// </remarks>
        public static CacheKey PicturesByVirtualPathCacheKey => new CacheKey("TVProgViewer.pictures.virtualpath-{0}");
    }
}