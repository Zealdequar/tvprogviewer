using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents an order note cache event consumer
    /// </summary>
    public partial class OrderNoteCacheEventConsumer : CacheEventConsumer<OrderNote>
    {
    }
}
