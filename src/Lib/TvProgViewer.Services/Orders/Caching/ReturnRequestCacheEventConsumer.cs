using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a return request cache event consumer
    /// </summary>
    public partial class ReturnRequestCacheEventConsumer : CacheEventConsumer<ReturnRequest>
    {
    }
}
