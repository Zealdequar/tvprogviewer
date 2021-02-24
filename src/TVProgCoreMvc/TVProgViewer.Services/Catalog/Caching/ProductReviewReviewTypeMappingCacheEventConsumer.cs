using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
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
        protected override async Task ClearCacheAsync(ProductReviewReviewTypeMapping entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.ProductReviewTypeMappingByReviewTypeCacheKey, entity.ProductReviewId);
        }
    }
}
