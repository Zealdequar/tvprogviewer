using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tier price cache event consumer
    /// </summary>
    public partial class TierPriceCacheEventConsumer : CacheEventConsumer<TierPrice>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(TierPrice entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.TierPricesByProductCacheKey, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductPricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
        }
    }
}
