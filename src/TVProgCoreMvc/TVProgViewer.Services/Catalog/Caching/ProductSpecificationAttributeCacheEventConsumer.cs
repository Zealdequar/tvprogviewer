using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product specification attribute cache event consumer
    /// </summary>
    public partial class ProductSpecificationAttributeCacheEventConsumer : CacheEventConsumer<ProductSpecificationAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(ProductSpecificationAttribute entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductSpecificationAttributeByProductPrefix, entity.ProductId);
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeGroupByProductCacheKey, entity.ProductId);
        }
    }
}
