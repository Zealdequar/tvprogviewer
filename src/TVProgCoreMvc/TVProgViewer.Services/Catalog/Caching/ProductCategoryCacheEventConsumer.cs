using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product category cache event consumer
    /// </summary>
    public partial class ProductCategoryCacheEventConsumer : CacheEventConsumer<ProductCategory>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(ProductCategory entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductCategoriesByProductPrefix, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoryProductsNumberPrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductPricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoryFeaturedProductsIdsPrefix, entity.CategoryId);
        }
    }
}
