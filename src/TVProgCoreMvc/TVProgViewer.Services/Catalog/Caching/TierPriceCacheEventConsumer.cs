using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
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
        protected override async Task ClearCacheAsync(TierPrice entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.TierPricesByProductCacheKey, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductPricePrefix, entity.ProductId);
        }
    }
}
