using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Common.Caching
{
    /// <summary>
    /// Represents a address attribute cache event consumer
    /// </summary>
    public partial class AddressAttributeCacheEventConsumer : CacheEventConsumer<AddressAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(AddressAttribute entity)
        {
            await RemoveAsync(TvProgCommonDefaults.AddressAttributeValuesByAttributeCacheKey, entity);
        }
    }
}
