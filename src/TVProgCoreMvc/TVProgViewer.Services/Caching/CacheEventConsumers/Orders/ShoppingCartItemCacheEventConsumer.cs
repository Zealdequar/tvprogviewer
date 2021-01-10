using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a shopping cart item cache event consumer
    /// </summary>
    public partial class ShoppingCartItemCacheEventConsumer : CacheEventConsumer<ShoppingCartItem>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ShoppingCartItem entity)
        {
            RemoveByPrefix(TvProgOrderCachingDefaults.ShoppingCartPrefixCacheKey, false);
        }
    }
}
