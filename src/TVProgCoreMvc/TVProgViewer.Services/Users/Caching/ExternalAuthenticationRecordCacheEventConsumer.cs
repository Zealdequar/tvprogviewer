using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Users.Caching
{
    /// <summary>
    /// Represents an external authentication record cache event consumer
    /// </summary>
    public partial class ExternalAuthenticationRecordCacheEventConsumer : CacheEventConsumer<ExternalAuthenticationRecord>
    {
    }
}
