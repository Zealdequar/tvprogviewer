using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Forums.Caching
{
    /// <summary>
    /// Represents a private message cache event consumer
    /// </summary>
    public partial class PrivateMessageCacheEventConsumer : CacheEventConsumer<PrivateMessage>
    {
    }
}
