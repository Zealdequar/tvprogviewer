using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Common.Caching
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(AddressAttribute entity)
        {
            await RemoveAsync(TvProgCommonDefaults.AddressAttributeValuesByAttributeCacheKey, entity);
        }
    }
}
