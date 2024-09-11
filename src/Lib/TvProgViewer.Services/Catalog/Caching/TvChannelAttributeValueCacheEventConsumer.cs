using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvChannel attribute value cache event consumer
    /// </summary>
    public partial class TvChannelAttributeValueCacheEventConsumer : CacheEventConsumer<TvChannelAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannelAttributeValue entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.TvChannelAttributeValuesByAttributeCacheKey, entity.TvChannelAttributeMappingId);
        }
    }
}
