using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product review helpfulness cache event consumer
    /// </summary>
    public partial class ProductReviewHelpfulnessCacheEventConsumer : CacheEventConsumer<ProductReviewHelpfulness>
    {
    }
}
