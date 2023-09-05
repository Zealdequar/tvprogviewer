using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Messages.Caching
{
    /// <summary>
    /// Represents a campaign cache event consumer
    /// </summary>
    public partial class CampaignCacheEventConsumer : CacheEventConsumer<Campaign>
    {
    }
}
