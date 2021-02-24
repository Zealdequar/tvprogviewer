using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipping method-country mapping cache event consumer
    /// </summary>
    public partial class ShippingMethodCountryMappingCacheEventConsumer : CacheEventConsumer<ShippingMethodCountryMapping>
    {
    }
}