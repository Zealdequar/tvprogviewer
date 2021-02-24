using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Gdpr;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Gdpr.Caching
{
    /// <summary>
    /// Represents a GDPR consent cache event consumer
    /// </summary>
    public partial class GdprConsentCacheEventConsumer : CacheEventConsumer<GdprConsent>
    {
    }
}