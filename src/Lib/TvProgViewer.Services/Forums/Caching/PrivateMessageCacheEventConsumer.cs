using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Forums.Caching
{
    /// <summary>
    /// Represents a private message cache event consumer
    /// </summary>
    public partial class PrivateMessageCacheEventConsumer : CacheEventConsumer<PrivateMessage>
    {
    }
}
