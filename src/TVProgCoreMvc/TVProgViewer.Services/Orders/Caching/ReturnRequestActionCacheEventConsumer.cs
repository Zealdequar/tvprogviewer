using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Orders.Caching
{
    /// <summary>
    /// Represents a return request action cache event consumer
    /// </summary>
    public partial class ReturnRequestActionCacheEventConsumer : CacheEventConsumer<ReturnRequestAction>
    {
    }
}
