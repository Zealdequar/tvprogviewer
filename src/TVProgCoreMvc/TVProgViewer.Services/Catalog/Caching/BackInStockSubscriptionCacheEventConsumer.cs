using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a back in stock subscription cache event consumer
    /// </summary>
    public partial class BackInStockSubscriptionCacheEventConsumer : CacheEventConsumer<BackInStockSubscription>
    {
    }
}
