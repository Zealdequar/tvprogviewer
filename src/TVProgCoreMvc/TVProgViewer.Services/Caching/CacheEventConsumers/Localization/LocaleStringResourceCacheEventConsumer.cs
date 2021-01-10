using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Localization
{
    /// <summary>
    /// Represents a locale string resource cache event consumer
    /// </summary>
    public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(LocaleStringResource entity)
        {
            RemoveByPrefix(TvProgLocalizationCachingDefaults.LocaleStringResourcesPrefixCacheKey);
        }
    }
}
