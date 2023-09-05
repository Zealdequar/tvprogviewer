using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipment item cache event consumer
    /// </summary>
    public partial class ShipmentItemCacheEventConsumer : CacheEventConsumer<ShipmentItem>
    {
    }
}