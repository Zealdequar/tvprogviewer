﻿using TvProgViewer.Core.Domain.Polls;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Polls.Caching
{
    /// <summary>
    /// Represents a poll voting record cache event consumer
    /// </summary>
    public partial class PollVotingRecordCacheEventConsumer : CacheEventConsumer<PollVotingRecord>
    {
    }
}