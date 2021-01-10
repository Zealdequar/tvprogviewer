using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product attribute value cache event consumer
    /// </summary>
    public partial class ProductAttributeValueCacheEventConsumer : CacheEventConsumer<ProductAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductAttributeValue entity)
        {
            RemoveByPrefix(TvProgCatalogCachingDefaults.ProductAttributesAllPrefixCacheKey);
            RemoveByPrefix(TvProgCatalogCachingDefaults.ProductAttributeValuesAllPrefixCacheKey);
        }
    }
}
