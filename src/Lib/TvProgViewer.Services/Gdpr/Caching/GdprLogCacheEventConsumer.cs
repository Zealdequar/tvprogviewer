using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Gdpr.Caching
{
    /// <summary>
    /// Represents a GDPR log cache event consumer
    /// </summary>
    public partial class GdprLogCacheEventConsumer : CacheEventConsumer<GdprLog>
    {
    }
}
