using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Directory.Caching
{
    /// <summary>
    /// Represents a currency cache event consumer
    /// </summary>
    public partial class CurrencyCacheEventConsumer : CacheEventConsumer<Currency>
    {
    }
}
