using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum topic cache event consumer
    /// </summary>
    public partial class ForumTopicCacheEventConsumer : CacheEventConsumer<ForumTopic>
    {
    }
}
