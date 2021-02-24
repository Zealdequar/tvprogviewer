using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Common.Caching
{
    /// <summary>
    /// Represents a generic attribute cache event consumer
    /// </summary>
    public partial class GenericAttributeCacheEventConsumer : CacheEventConsumer<GenericAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(GenericAttribute entity)
        {
            await RemoveAsync(TvProgCommonDefaults.GenericAttributeCacheKey, entity.EntityId, entity.KeyGroup);
        }
    }
}
