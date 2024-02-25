using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a predefined tvchannel attribute value cache event consumer
    /// </summary>
    public partial class PredefinedTvChannelAttributeValueCacheEventConsumer : CacheEventConsumer<PredefinedTvChannelAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(PredefinedTvChannelAttributeValue entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.PredefinedTvChannelAttributeValuesByAttributeCacheKey, entity.TvChannelAttributeId);
        }
    }
}
