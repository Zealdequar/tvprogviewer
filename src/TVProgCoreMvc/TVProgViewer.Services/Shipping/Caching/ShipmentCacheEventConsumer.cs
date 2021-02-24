using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipment cache event consumer
    /// </summary>
    public partial class ShipmentCacheEventConsumer : CacheEventConsumer<Shipment>
    {
    }
}