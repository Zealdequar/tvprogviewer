using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
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
        protected override async Task ClearCacheAsync(ProductAttributeMapping entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.ProductAttributeMappingsByProductCacheKey, entity.ProductId);
            await RemoveAsync(TvProgCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity);
            await RemoveAsync(TvProgCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, entity.ProductId);
        }
    }
}
