using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Users.Caching
{
    /// <summary>
    /// Represents a reward point history cache event consumer
    /// </summary>
    public partial class RewardPointsHistoryCacheEventConsumer : CacheEventConsumer<RewardPointsHistory>
    {
    }
}
