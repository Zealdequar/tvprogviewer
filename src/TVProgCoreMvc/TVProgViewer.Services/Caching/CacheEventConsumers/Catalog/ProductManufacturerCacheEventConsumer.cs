using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
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
        protected override void ClearCache(ProductManufacturer entity)
        {
            var prefix = TvProgCatalogCachingDefaults.ProductManufacturersByProductPrefixCacheKey.ToCacheKey(entity.ProductId);
            RemoveByPrefix(prefix);

            prefix = TvProgCatalogCachingDefaults.ProductManufacturersByManufacturerPrefixCacheKey.ToCacheKey(entity.ManufacturerId);
            RemoveByPrefix(prefix);

            prefix = TvProgCatalogCachingDefaults.ProductPricePrefixCacheKey.ToCacheKey(entity.ProductId);
            RemoveByPrefix(prefix);
        }
    }
}
