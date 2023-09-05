using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Services.Caching;
using TvProgViewer.Services.Localization;
using System.Threading.Tasks;

namespace TvProgViewer.Services.Stores.Caching
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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Store entity)
        {
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            await RemoveByPrefixAsync(TvProgLocalizationDefaults.LanguagesByStorePrefix, entity);
        }
    }
}
