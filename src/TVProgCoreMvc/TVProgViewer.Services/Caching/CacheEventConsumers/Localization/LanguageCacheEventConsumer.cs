using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Localization
{
    /// <summary>
    /// Represents a language cache event consumer
    /// </summary>
    public partial class LanguageCacheEventConsumer : CacheEventConsumer<Language>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Language entity)
        {
            Remove(TvProgLocalizationCachingDefaults.LocaleStringResourcesAllPublicCacheKey.FillCacheKey(entity));
            Remove(TvProgLocalizationCachingDefaults.LocaleStringResourcesAllAdminCacheKey.FillCacheKey(entity));
            Remove(TvProgLocalizationCachingDefaults.LocaleStringResourcesAllCacheKey.FillCacheKey(entity));

            var prefix = TvProgLocalizationCachingDefaults.LocaleStringResourcesByResourceNamePrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(TvProgLocalizationCachingDefaults.LanguagesAllPrefixCacheKey);

        }
    }
}