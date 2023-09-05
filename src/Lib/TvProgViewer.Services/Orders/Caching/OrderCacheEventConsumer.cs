using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a order cache event consumer
    /// </summary>
    public partial class OrderCacheEventConsumer : CacheEventConsumer<Order>
    {
    }
}