using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a recurring payment cache event consumer
    /// </summary>
    public partial class RecurringPaymentCacheEventConsumer : CacheEventConsumer<RecurringPayment>
    { 
    }
}
