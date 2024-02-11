using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvchannel review helpfulness cache event consumer
    /// </summary>
    public partial class TvChannelReviewHelpfulnessCacheEventConsumer : CacheEventConsumer<TvChannelReviewHelpfulness>
    {
    }
}
