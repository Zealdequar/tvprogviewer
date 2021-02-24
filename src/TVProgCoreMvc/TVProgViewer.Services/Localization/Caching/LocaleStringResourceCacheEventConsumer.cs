using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Localization.Caching
{
    /// <summary>
    /// Represents a locale string resource cache event consumer
    /// </summary>
    public partial class LocaleStringResourceCacheEventConsumer : CacheEventConsumer<LocaleStringResource>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(LocaleStringResource entity)
        {
            await RemoveAsync(TvProgLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, entity.LanguageId);
            await RemoveAsync(TvProgLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, entity.LanguageId);
            await RemoveAsync(TvProgLocalizationDefaults.LocaleStringResourcesAllCacheKey, entity.LanguageId);
            await RemoveByPrefixAsync(TvProgLocalizationDefaults.LocaleStringResourcesByNamePrefix, entity.LanguageId);
        }
    }
}
