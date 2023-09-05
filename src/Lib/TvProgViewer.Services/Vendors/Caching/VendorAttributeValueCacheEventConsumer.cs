using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TvProgViewer.Services.Vendors.Caching
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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(VendorAttributeValue entity)
        {
            await RemoveAsync(TvProgVendorDefaults.VendorAttributeValuesByAttributeCacheKey, entity.VendorAttributeId);
        }
    }
}
