using TVProgViewer.Core.Domain.Seo;
using TVProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Seo.Caching
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
        protected override async Task ClearCacheAsync(UrlRecord entity)
        {
            await RemoveAsync(TvProgSeoDefaults.UrlRecordCacheKey, entity.EntityId, entity.EntityName, entity.LanguageId);
            await RemoveAsync(TvProgSeoDefaults.UrlRecordBySlugCacheKey, entity.Slug);
        }
    }
}
