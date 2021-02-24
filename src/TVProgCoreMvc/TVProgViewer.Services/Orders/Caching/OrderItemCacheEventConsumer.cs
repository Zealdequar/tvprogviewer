using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents an order item cache event consumer
    /// </summary>
    public partial class OrderItemCacheEventConsumer : CacheEventConsumer<OrderItem>
    {
    }
}
