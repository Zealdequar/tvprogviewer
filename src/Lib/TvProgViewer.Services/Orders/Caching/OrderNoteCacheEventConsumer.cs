using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents an order note cache event consumer
    /// </summary>
    public partial class OrderNoteCacheEventConsumer : CacheEventConsumer<OrderNote>
    {
    }
}
