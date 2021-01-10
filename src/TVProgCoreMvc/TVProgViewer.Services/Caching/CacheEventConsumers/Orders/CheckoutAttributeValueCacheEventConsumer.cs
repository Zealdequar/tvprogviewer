using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a checkout attribute value cache event consumer
    /// </summary>
    public partial class CheckoutAttributeValueCacheEventConsumer : CacheEventConsumer<CheckoutAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CheckoutAttributeValue entity)
        {
            var cacheKey = TvProgOrderCachingDefaults.CheckoutAttributeValuesAllCacheKey.FillCacheKey(entity.CheckoutAttributeId);
            Remove(cacheKey);

        }
    }
}
