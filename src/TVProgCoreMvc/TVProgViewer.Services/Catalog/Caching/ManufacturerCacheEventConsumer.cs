using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;
using TVProgViewer.Services.Discounts;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a manufacturer cache event consumer
    /// </summary>
    public partial class ManufacturerCacheEventConsumer : CacheEventConsumer<Manufacturer>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Manufacturer entity)
        {
            await RemoveByPrefixAsync(TvProgDiscountDefaults.ManufacturerIdsPrefix);
        }
    }
}
