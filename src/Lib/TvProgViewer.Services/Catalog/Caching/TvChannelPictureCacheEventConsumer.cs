using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvChannel picture mapping cache event consumer
    /// </summary>
    public partial class TvChannelPictureCacheEventConsumer : CacheEventConsumer<TvChannelPicture>
    {
    }
}
