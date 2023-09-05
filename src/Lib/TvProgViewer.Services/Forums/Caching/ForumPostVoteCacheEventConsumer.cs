using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum post vote cache event consumer
    /// </summary>
    public partial class ForumPostVoteCacheEventConsumer : CacheEventConsumer<ForumPostVote>
    {
    }
}
