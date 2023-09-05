using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Common.Caching
{
    /// <summary>
    /// Represents a search term cache event consumer
    /// </summary>
    public partial class SearchTermCacheEventConsumer : CacheEventConsumer<SearchTerm>
    {
    }
}
