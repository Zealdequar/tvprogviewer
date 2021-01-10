using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a delivery date cache event consumer
    /// </summary>
    public partial class DeliveryDateCacheEventConsumer : CacheEventConsumer<DeliveryDate>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(DeliveryDate entity)
        {
            Remove(TvProgShippingCachingDefaults.DeliveryDatesAllCacheKey);
        }

    }
}