using TVProgViewer.Core.Domain.Seo;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Seo
{
    /// <summary>
    /// Represents an URL record cache event consumer
    /// </summary>
    public partial class UrlRecordCacheEventConsumer : CacheEventConsumer<UrlRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(UrlRecord entity)
        {
            Remove(TvProgSeoCachingDefaults.UrlRecordAllCacheKey);

            var cacheKey = TvProgSeoCachingDefaults.UrlRecordActiveByIdNameLanguageCacheKey.FillCacheKey(entity.EntityId,
                entity.EntityName, entity.LanguageId);
            Remove(cacheKey);

            RemoveByPrefix(TvProgSeoCachingDefaults.UrlRecordByIdsPrefixCacheKey);

            cacheKey = TvProgSeoCachingDefaults.UrlRecordBySlugCacheKey.FillCacheKey(entity.Slug);
            Remove(cacheKey);
        }
    }
}
