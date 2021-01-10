using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product attribute cache event consumer
    /// </summary>
    public partial class ProductAttributeCacheEventConsumer : CacheEventConsumer<ProductAttribute>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(ProductAttribute entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
            {
                RemoveByPrefix(TvProgCatalogCachingDefaults.ProductAttributeMappingsPrefixCacheKey);
                RemoveByPrefix(TvProgCatalogCachingDefaults.ProductAttributeValuesAllPrefixCacheKey);
                RemoveByPrefix(TvProgCatalogCachingDefaults.ProductAttributeCombinationsAllPrefixCacheKey);
            }

            RemoveByPrefix(TvProgCatalogCachingDefaults.ProductAttributesAllPrefixCacheKey);

            var cacheKey = TvProgCatalogCachingDefaults.ProductsByProductAtributeCacheKey.FillCacheKey(entity);
            Remove(cacheKey);
        }
    }
}
