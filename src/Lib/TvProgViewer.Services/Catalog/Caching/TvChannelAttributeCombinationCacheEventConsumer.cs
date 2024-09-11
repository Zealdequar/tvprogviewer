using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvChannel attribute combination cache event consumer
    /// </summary>
    public partial class TvChannelAttributeCombinationCacheEventConsumer : CacheEventConsumer<TvChannelAttributeCombination>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannelAttributeCombination entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.TvChannelAttributeMappingsByTvChannelCacheKey, entity.TvChannelId);
            await RemoveAsync(TvProgCatalogDefaults.TvChannelAttributeCombinationsByTvChannelCacheKey, entity.TvChannelId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelMultiplePricePrefix, entity.TvChannelId);
        }
    }
}
