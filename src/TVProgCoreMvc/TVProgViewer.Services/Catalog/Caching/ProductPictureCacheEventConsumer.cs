using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product picture mapping cache event consumer
    /// </summary>
    public partial class ProductPictureCacheEventConsumer : CacheEventConsumer<ProductPicture>
    {
    }
}
