using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute combination cache event consumer
    /// </summary>
    public partial class ProductAttributeCombinationCacheEventConsumer : CacheEventConsumer<ProductAttributeCombination>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(ProductAttributeCombination entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.ProductAttributeMappingsByProductCacheKey, entity.ProductId);
            await RemoveAsync(TvProgCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, entity.ProductId);
        }
    }
}
