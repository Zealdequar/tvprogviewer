using TvProgViewer.Core.Domain.ScheduleTasks;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.ScheduleTasks.Caching
{
    /// <summary>
    /// Represents a schedule task cache event consumer
    /// </summary>
    public partial class ScheduleTaskCacheEventConsumer : CacheEventConsumer<ScheduleTask>
    {
    }
}
