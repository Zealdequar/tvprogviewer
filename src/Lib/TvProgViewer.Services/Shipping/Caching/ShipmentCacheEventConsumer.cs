using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipment cache event consumer
    /// </summary>
    public partial class ShipmentCacheEventConsumer : CacheEventConsumer<Shipment>
    {
    }
}