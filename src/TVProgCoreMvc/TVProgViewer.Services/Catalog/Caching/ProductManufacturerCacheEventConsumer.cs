using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product manufacturer cache event consumer
    /// </summary>
    public partial class ProductManufacturerCacheEventConsumer : CacheEventConsumer<ProductManufacturer>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(ProductManufacturer entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductManufacturersByProductPrefix, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductPricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ManufacturerFeaturedProductIdsPrefix, entity.ManufacturerId);
        }
    }
}
