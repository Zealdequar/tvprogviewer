using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Orders.Caching
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
        protected override async Task ClearCacheAsync(CheckoutAttribute entity)
        {
            await RemoveAsync(TvProgOrderDefaults.CheckoutAttributeValuesAllCacheKey, entity);
        }
    }
}
