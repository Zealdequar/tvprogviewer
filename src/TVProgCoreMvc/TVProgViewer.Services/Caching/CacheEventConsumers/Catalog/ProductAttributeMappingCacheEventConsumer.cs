using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
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
        protected override void ClearCache(ProductAttributeMapping entity)
        {
            var cacheKey = TvProgCatalogCachingDefaults.ProductsByProductAtributeCacheKey.FillCacheKey(entity.ProductAttributeId);
            Remove(cacheKey);

            cacheKey = TvProgCatalogCachingDefaults.ProductAttributeMappingsAllCacheKey.FillCacheKey(entity.ProductId);
            Remove(cacheKey);

            cacheKey = TvProgCatalogCachingDefaults.ProductAttributeValuesAllCacheKey.FillCacheKey(entity);
            Remove(cacheKey);

            RemoveByPrefix(TvProgCatalogCachingDefaults.ProductAttributesAllPrefixCacheKey);

            cacheKey = TvProgCatalogCachingDefaults.ProductAttributeCombinationsAllCacheKey.FillCacheKey(entity.ProductId);
            Remove(cacheKey);
        }
    }
}
