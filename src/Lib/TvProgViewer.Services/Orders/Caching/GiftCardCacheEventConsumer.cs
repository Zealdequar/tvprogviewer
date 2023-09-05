using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a gift card cache event consumer
    /// </summary>
    public partial class GiftCardCacheEventConsumer : CacheEventConsumer<GiftCard>
    {
    }
}
