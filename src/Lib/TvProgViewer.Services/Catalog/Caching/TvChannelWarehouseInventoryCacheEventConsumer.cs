using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvChannel warehouse inventory cache event consumer
    /// </summary>
    public partial class TvChannelWarehouseInventoryCacheEventConsumer : CacheEventConsumer<TvChannelWarehouseInventory>
    {
    }
}
