using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a shipping method cache event consumer
    /// </summary>
    public partial class ShippingMethodCacheEventConsumer : CacheEventConsumer<ShippingMethod>
    {
        protected override void ClearCache(ShippingMethod entity)
        {
            RemoveByPrefix(TvProgShippingCachingDefaults.ShippingMethodsAllPrefixCacheKey);
        }
    }
}
