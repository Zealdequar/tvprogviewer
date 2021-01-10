using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
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
        protected override void ClearCache(Product entity)
        {
            var prefix = TvProgCatalogCachingDefaults.ProductManufacturersByProductPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            Remove(TvProgCatalogCachingDefaults.ProductsAllDisplayedOnHomepageCacheKey);
            RemoveByPrefix(TvProgCatalogCachingDefaults.ProductsByIdsPrefixCacheKey);

            prefix = TvProgCatalogCachingDefaults.ProductPricePrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(TvProgOrderCachingDefaults.ShoppingCartPrefixCacheKey);
        }
    }
}
