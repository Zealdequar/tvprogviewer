using TVProgViewer.Core.Domain.Messages;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Messages
{
    /// <summary>
    /// Represents an queued email cache event consumer
    /// </summary>
    public partial class QueuedEmailCacheEventConsumer : CacheEventConsumer<QueuedEmail>
    {
    }
}
