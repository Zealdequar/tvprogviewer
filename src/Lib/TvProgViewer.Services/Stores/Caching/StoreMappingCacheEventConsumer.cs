using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Stores.Caching
{
    /// <summary>
    /// Represents a store mapping cache event consumer
    /// </summary>
    public partial class StoreMappingCacheEventConsumer : CacheEventConsumer<StoreMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(StoreMapping entity)
        {
            await RemoveAsync(TvProgStoreDefaults.StoreMappingsCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(TvProgStoreDefaults.StoreMappingIdsCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(TvProgStoreDefaults.StoreMappingExistsCacheKey, entity.EntityName);
        }
    }
}
