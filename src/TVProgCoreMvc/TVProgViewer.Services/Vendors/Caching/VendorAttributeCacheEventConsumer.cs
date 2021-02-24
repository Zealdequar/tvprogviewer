using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Vendors.Caching
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
        protected override async Task ClearCacheAsync(VendorAttribute entity)
        {
            await RemoveAsync(TvProgVendorDefaults.VendorAttributeValuesByAttributeCacheKey, entity);
        }
    }
}
