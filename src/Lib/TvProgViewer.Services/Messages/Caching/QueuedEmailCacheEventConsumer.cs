using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Messages.Caching
{
    /// <summary>
    /// Represents an queued email cache event consumer
    /// </summary>
    public partial class QueuedEmailCacheEventConsumer : CacheEventConsumer<QueuedEmail>
    {
    }
}
