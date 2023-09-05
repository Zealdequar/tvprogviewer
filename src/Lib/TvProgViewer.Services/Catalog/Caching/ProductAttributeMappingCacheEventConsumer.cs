using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute mapping cache event consumer
    /// </summary>
    public partial class ProductAttributeMappingCacheEventConsumer : CacheEventConsumer<ProductAttributeMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductAttributeMapping entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.ProductAttributeMappingsByProductCacheKey, entity.ProductId);
            await RemoveAsync(TvProgCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity);
            await RemoveAsync(TvProgCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
        }
    }
}
