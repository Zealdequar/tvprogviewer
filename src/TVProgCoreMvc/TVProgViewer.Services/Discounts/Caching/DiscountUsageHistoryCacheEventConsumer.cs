using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount usage history cache event consumer
    /// </summary>
    public partial class DiscountUsageHistoryCacheEventConsumer : CacheEventConsumer<DiscountUsageHistory>
    {
    }
}
