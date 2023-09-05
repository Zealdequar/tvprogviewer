using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product review cache event consumer
    /// </summary>
    public partial class ProductReviewCacheEventConsumer : CacheEventConsumer<ProductReview>
    {
    }
}
