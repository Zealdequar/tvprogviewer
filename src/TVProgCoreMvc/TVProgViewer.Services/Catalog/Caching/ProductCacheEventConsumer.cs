using System.Threading.Tasks;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product cache event consumer
    /// </summary>
    public partial class ProductCacheEventConsumer : CacheEventConsumer<Product>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Product entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductManufacturersByProductPrefix, entity);
            await RemoveAsync(TvProgCatalogDefaults.ProductsHomepageCacheKey);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductPricePrefix, entity);
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.FeaturedProductIdsPrefix);
        }
    }
}
