using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a gift card cache event consumer
    /// </summary>
    public partial class GiftCardCacheEventConsumer : CacheEventConsumer<GiftCard>
    {
    }
}
