using TvProgViewer.Core.Domain.Polls;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Polls.Caching
{
    /// <summary>
    /// Represents a poll answer cache event consumer
    /// </summary>
    public partial class PollAnswerCacheEventConsumer : CacheEventConsumer<PollAnswer>
    {
    }
}