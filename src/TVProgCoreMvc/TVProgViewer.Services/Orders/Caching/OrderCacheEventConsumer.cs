using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a order cache event consumer
    /// </summary>
    public partial class OrderCacheEventConsumer : CacheEventConsumer<Order>
    {
    }
}