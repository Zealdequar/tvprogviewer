using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a return request reason cache event consumer
    /// </summary>
    public partial class ReturnRequestReasonCacheEventConsumer : CacheEventConsumer<ReturnRequestReason>
    {
        protected override void ClearCache(ReturnRequestReason entity)
        {
            Remove(TvProgOrderCachingDefaults.ReturnRequestReasonAllCacheKey);
        }

    }
}
