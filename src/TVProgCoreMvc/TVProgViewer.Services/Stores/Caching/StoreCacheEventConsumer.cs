using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Services.Caching;
using TVProgViewer.Services.Localization;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Stores.Caching
{
    /// <summary>
    /// Represents a store cache event consumer
    /// </summary>
    public partial class StoreCacheEventConsumer : CacheEventConsumer<Store>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Store entity)
        {
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            await RemoveByPrefixAsync(TvProgLocalizationDefaults.LanguagesByStorePrefix, entity);
        }
    }
}
