using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Polls.Caching
{
    /// <summary>
    /// Represents a poll answer cache event consumer
    /// </summary>
    public partial class PollAnswerCacheEventConsumer : CacheEventConsumer<PollAnswer>
    {
    }
}