using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum subscription cache event consumer
    /// </summary>
    public partial class ForumSubscriptionCacheEventConsumer : CacheEventConsumer<ForumSubscription>
    {
    }
}
