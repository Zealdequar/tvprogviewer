using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Messages.Caching
{
    /// <summary>
    /// Represents a campaign cache event consumer
    /// </summary>
    public partial class CampaignCacheEventConsumer : CacheEventConsumer<Campaign>
    {
    }
}
