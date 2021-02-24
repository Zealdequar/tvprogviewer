using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum post vote cache event consumer
    /// </summary>
    public partial class ForumPostVoteCacheEventConsumer : CacheEventConsumer<ForumPostVote>
    {
    }
}
