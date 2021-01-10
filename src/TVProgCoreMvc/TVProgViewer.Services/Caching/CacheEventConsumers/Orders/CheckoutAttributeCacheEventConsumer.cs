using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a checkout attribute cache event consumer
    /// </summary>
    public partial class CheckoutAttributeCacheEventConsumer : CacheEventConsumer<CheckoutAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CheckoutAttribute entity)
        {
            RemoveByPrefix(TvProgOrderCachingDefaults.CheckoutAttributesAllPrefixCacheKey);
            var cacheKey = TvProgOrderCachingDefaults.CheckoutAttributeValuesAllCacheKey.FillCacheKey(entity);
            Remove(cacheKey);
        }
    }
}
