using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Shipping
{
    /// <summary>
    /// Represents a warehouse cache event consumer
    /// </summary>
    public partial class WarehouseCacheEventConsumer : CacheEventConsumer<Warehouse>
    {
        protected override void ClearCache(Warehouse entity)
        {
            Remove(TvProgShippingCachingDefaults.WarehousesAllCacheKey);
        }
    }
}
