using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Directory.Caching
{
    /// <summary>
    /// Represents a currency cache event consumer
    /// </summary>
    public partial class CurrencyCacheEventConsumer : CacheEventConsumer<Currency>
    {
    }
}
