using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Directory.Caching
{
    /// <summary>
    /// Represents a measure weight cache event consumer
    /// </summary>
    public partial class MeasureWeightCacheEventConsumer : CacheEventConsumer<MeasureWeight>
    {
    }
}
