using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Localization
{
    /// <summary>
    /// Represents a localized property cache event consumer
    /// </summary>
    public partial class LocalizedPropertyCacheEventConsumer : CacheEventConsumer<LocalizedProperty>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(LocalizedProperty entity)
        {
            Remove(TvProgLocalizationCachingDefaults.LocalizedPropertyAllCacheKey);

            var cacheKey = TvProgLocalizationCachingDefaults.LocalizedPropertyCacheKey
                .FillCacheKey(entity.LanguageId, entity, entity.LocaleKeyGroup, entity.LocaleKey);

            Remove(cacheKey);

        }
    }
}
