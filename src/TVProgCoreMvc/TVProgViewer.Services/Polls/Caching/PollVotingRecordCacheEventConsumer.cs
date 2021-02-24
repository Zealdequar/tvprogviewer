using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Polls.Caching
{
    /// <summary>
    /// Represents a poll voting record cache event consumer
    /// </summary>
    public partial class PollVotingRecordCacheEventConsumer : CacheEventConsumer<PollVotingRecord>
    {
    }
}