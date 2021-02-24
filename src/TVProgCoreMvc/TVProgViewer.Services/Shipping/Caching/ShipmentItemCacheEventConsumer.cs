using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipment item cache event consumer
    /// </summary>
    public partial class ShipmentItemCacheEventConsumer : CacheEventConsumer<ShipmentItem>
    {
    }
}