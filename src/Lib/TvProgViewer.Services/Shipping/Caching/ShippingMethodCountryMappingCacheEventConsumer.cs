using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a shipping method-country mapping cache event consumer
    /// </summary>
    public partial class ShippingMethodCountryMappingCacheEventConsumer : CacheEventConsumer<ShippingMethodCountryMapping>
    {
    }
}