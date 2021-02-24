using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a recurring payment history cache event consumer
    /// </summary>
    public partial class RecurringPaymentHistoryCacheEventConsumer : CacheEventConsumer<RecurringPaymentHistory>
    {
    }
}
