using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a cross-sell tvChannel cache event consumer
    /// </summary>
    public partial class CrossSellTvChannelCacheEventConsumer : CacheEventConsumer<CrossSellTvChannel>
    {
    }
}
