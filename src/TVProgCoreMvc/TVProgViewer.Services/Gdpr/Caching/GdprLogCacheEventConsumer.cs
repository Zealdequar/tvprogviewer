using TVProgViewer.Core.Domain.Gdpr;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Gdpr.Caching
{
    /// <summary>
    /// Represents a GDPR log cache event consumer
    /// </summary>
    public partial class GdprLogCacheEventConsumer : CacheEventConsumer<GdprLog>
    {
    }
}
