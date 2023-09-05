using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Directory.Caching
{
    /// <summary>
    /// Represents a measure dimension cache event consumer
    /// </summary>
    public partial class MeasureDimensionCacheEventConsumer : CacheEventConsumer<MeasureDimension>
    {
    }
}
