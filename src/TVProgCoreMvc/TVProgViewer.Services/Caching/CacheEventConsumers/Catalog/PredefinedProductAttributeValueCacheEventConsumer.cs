using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a predefined product attribute value cache event consumer
    /// </summary>
    public partial class PredefinedProductAttributeValueCacheEventConsumer : CacheEventConsumer<PredefinedProductAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(PredefinedProductAttributeValue entity)
        {
            RemoveByPrefix(TvProgCatalogCachingDefaults.ProductAttributesAllPrefixCacheKey);

            var cacheKey = TvProgCatalogCachingDefaults.PredefinedProductAttributeValuesAllCacheKey.FillCacheKey(entity.ProductAttributeId);
            Remove(cacheKey);
        }
    }
}
