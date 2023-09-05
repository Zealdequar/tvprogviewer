using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Gdpr.Caching
{
    /// <summary>
    /// Represents a GDPR consent cache event consumer
    /// </summary>
    public partial class GdprConsentCacheEventConsumer : CacheEventConsumer<GdprConsent>
    {
    }
}