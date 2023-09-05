using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(ProductManufacturer entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductManufacturersByProductPrefix, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductPricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ManufacturerFeaturedProductIdsPrefix, entity.ManufacturerId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ManufacturersByCategoryPrefix);
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, entity.ManufacturerId.ToString());
        }
    }
}
