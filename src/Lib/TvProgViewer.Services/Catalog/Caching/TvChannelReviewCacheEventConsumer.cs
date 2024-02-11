using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvchannel review cache event consumer
    /// </summary>
    public partial class TvChannelReviewCacheEventConsumer : CacheEventConsumer<TvChannelReview>
    {
    }
}
