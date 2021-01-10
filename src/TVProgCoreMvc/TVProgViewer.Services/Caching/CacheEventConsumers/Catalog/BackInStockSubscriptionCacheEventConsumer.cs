using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CacheEventConsumers;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a back in stock subscription cache event consumer
    /// </summary>
    public partial class BackInStockSubscriptionCacheEventConsumer : CacheEventConsumer<BackInStockSubscription>
    {
    }
}
