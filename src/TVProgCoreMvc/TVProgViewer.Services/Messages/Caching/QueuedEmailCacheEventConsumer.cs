using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Messages.Caching
{
    /// <summary>
    /// Represents an queued email cache event consumer
    /// </summary>
    public partial class QueuedEmailCacheEventConsumer : CacheEventConsumer<QueuedEmail>
    {
    }
}
