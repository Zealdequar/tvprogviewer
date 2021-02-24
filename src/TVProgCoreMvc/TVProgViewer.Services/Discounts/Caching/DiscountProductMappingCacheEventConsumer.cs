using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-product mapping cache event consumer
    /// </summary>
    public partial class DiscountProductMappingCacheEventConsumer : CacheEventConsumer<DiscountManufacturerMapping>
    {
    }
}