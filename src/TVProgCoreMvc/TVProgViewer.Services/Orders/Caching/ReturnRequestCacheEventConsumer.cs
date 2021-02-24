using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a return request cache event consumer
    /// </summary>
    public partial class ReturnRequestCacheEventConsumer : CacheEventConsumer<ReturnRequest>
    {
    }
}
