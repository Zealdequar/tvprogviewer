using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a stock quantity change entry cache event consumer
    /// </summary>
    public partial class StockQuantityHistoryCacheEventConsumer : CacheEventConsumer<StockQuantityHistory>
    {
    }
}
