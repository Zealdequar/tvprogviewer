using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a product review review type cache event consumer
    /// </summary>
    public partial class ProductReviewReviewTypeMappingCacheEventConsumer : CacheEventConsumer<ProductReviewReviewTypeMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductReviewReviewTypeMapping entity)
        {
            Remove(TvProgCatalogCachingDefaults.ReviewTypeAllCacheKey);

            var cacheKey = TvProgCatalogCachingDefaults.ProductReviewReviewTypeMappingAllCacheKey.FillCacheKey(entity.ProductReviewId);
            Remove(cacheKey);
        }
    }
}
