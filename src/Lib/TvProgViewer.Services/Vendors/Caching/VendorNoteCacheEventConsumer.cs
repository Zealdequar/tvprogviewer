using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Vendors.Caching
{
    /// <summary>
    /// Represents a vendor note cache event consumer
    /// </summary>
    public partial class VendorNoteCacheEventConsumer : CacheEventConsumer<VendorNote>
    {
    }
}
