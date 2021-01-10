using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a return request action cache event consumer
    /// </summary>
    public partial class ReturnRequestActionCacheEventConsumer : CacheEventConsumer<ReturnRequestAction>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ReturnRequestAction entity)
        {
            Remove(TvProgOrderCachingDefaults.ReturnRequestActionAllCacheKey);
        }
    }
}
