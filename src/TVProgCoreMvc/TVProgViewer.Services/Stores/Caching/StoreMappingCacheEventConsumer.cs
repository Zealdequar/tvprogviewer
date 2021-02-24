using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Stores.Caching
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
        protected override async Task ClearCacheAsync(StoreMapping entity)
        {
            await RemoveAsync(TvProgStoreDefaults.StoreMappingsCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(TvProgStoreDefaults.StoreMappingIdsCacheKey, entity.EntityId, entity.EntityName);
            await RemoveAsync(TvProgStoreDefaults.StoreMappingExistsCacheKey, entity.StoreId, entity.EntityName);
        }
    }
}
