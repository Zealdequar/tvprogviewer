using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Vendors.Caching
{
    /// <summary>
    /// Represents a vendor attribute value cache event consumer
    /// </summary>
    public partial class VendorAttributeValueCacheEventConsumer : CacheEventConsumer<VendorAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(VendorAttributeValue entity)
        {
            await RemoveAsync(TvProgVendorDefaults.VendorAttributeValuesByAttributeCacheKey, entity.VendorAttributeId);
        }
    }
}
