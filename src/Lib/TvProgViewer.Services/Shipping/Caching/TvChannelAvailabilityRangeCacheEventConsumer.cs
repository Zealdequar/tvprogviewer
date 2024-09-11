using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Caching;
using System.Threading.Tasks;

namespace TvProgViewer.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a tvChannel availability range cache event consumer
    /// </summary>
    public partial class TvChannelAvailabilityRangeCacheEventConsumer : CacheEventConsumer<TvChannelAvailabilityRange>
    {
    }
}