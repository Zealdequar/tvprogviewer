using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TvProgViewer.Services.Vendors.Caching
{
    /// <summary>
    /// Represents a vendor attribute cache event consumer
    /// </summary>
    public partial class VendorAttributeCacheEventConsumer : CacheEventConsumer<VendorAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(VendorAttribute entity)
        {
            await RemoveAsync(TvProgVendorDefaults.VendorAttributeValuesByAttributeCacheKey, entity);
        }
    }
}
