using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Media
{
    /// <summary>
    /// Represents a picture cache event consumer
    /// </summary>
    public partial class PictureCacheEventConsumer : CacheEventConsumer<Picture>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Picture entity)
        {
            RemoveByPrefix(TvProgMediaCachingDefaults.ThumbsExistsPrefixCacheKey);

            var cacheKey = TvProgMediaCachingDefaults.PicturesByVirtualPathCacheKey.FillCacheKey(entity.VirtualPath);
            Remove(cacheKey);

        }
    }
}
