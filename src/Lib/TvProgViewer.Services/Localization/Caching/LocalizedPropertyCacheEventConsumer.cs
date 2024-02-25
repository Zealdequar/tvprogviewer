using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TvProgViewer.Services.Localization.Caching
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(LocalizedProperty entity)
        {
            await RemoveAsync(TvProgLocalizationDefaults.LocalizedPropertyCacheKey, entity.LanguageId, entity.EntityId, entity.LocaleKeyGroup, entity.LocaleKey);
            await RemoveAsync(TvProgLocalizationDefaults.LocalizedPropertiesCacheKey, entity.EntityId, entity.LocaleKeyGroup, entity.LocaleKey);
        }
    }
}
