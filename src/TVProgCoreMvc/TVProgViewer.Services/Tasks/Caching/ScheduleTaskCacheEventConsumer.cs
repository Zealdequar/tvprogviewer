using TVProgViewer.Core.Domain.Tasks;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Tasks.Caching
{
    /// <summary>
    /// Represents a schedule task cache event consumer
    /// </summary>
    public partial class ScheduleTaskCacheEventConsumer : CacheEventConsumer<ScheduleTask>
    {
    }
}
