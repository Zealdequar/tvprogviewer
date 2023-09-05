using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a recurring payment cache event consumer
    /// </summary>
    public partial class RecurringPaymentCacheEventConsumer : CacheEventConsumer<RecurringPayment>
    { 
    }
}
