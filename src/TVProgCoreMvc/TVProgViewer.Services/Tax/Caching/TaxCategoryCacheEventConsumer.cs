using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Tax.Caching
{
    /// <summary>
    /// Represents a TAX category cache event consumer
    /// </summary>
    public partial class TaxCategoryCacheEventConsumer : CacheEventConsumer<TaxCategory>
    {
    }
}
