using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product review cache event consumer
    /// </summary>
    public partial class ProductReviewCacheEventConsumer : CacheEventConsumer<ProductReview>
    {
    }
}
