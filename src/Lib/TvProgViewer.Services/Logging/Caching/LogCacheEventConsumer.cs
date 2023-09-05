using TvProgViewer.Core.Domain.Logging;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Logging.Caching
{
    /// <summary>
    /// Represents a log cache event consumer
    /// </summary>
    public partial class LogCacheEventConsumer : CacheEventConsumer<Log>
    {
    }
}
