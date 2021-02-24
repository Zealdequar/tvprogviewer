using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Vendors.Caching
{
    /// <summary>
    /// Represents a vendor note cache event consumer
    /// </summary>
    public partial class VendorNoteCacheEventConsumer : CacheEventConsumer<VendorNote>
    {
    }
}
