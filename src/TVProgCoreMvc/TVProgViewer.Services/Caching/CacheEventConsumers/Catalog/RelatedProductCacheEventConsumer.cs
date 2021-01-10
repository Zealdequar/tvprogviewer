using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a related product cache event consumer
    /// </summary>
    public partial class RelatedProductCacheEventConsumer : CacheEventConsumer<RelatedProduct>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(RelatedProduct entity)
        {
            var prefix = TvProgCatalogCachingDefaults.ProductsRelatedPrefixCacheKey.ToCacheKey(entity.ProductId1);
            RemoveByPrefix(prefix);
        }
    }
}
