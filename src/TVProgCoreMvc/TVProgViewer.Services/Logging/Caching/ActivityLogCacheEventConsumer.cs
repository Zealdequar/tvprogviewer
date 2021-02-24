using TVProgViewer.Core.Domain.Logging;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Logging.Caching
{
    /// <summary>
    /// Represents an activity log cache event consumer
    /// </summary>
    public partial class ActivityLogCacheEventConsumer : CacheEventConsumer<ActivityLog>
    {
    }
}