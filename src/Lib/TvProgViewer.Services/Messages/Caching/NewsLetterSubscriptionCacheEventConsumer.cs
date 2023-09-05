using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Messages.Caching
{
    /// <summary>
    /// Represents news letter subscription cache event consumer
    /// </summary>
    public partial class NewsLetterSubscriptionCacheEventConsumer : CacheEventConsumer<NewsLetterSubscription>
    {    
    }
}
