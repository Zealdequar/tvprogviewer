using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a stock quantity change entry cache event consumer
    /// </summary>
    public partial class StockQuantityHistoryCacheEventConsumer : CacheEventConsumer<StockQuantityHistory>
    {
    }
}
