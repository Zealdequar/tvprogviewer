using TvProgViewer.Core.Domain.Logging;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Logging.Caching
{
    /// <summary>
    /// Represents an activity log cache event consumer
    /// </summary>
    public partial class ActivityLogCacheEventConsumer : CacheEventConsumer<ActivityLog>
    {
    }
}