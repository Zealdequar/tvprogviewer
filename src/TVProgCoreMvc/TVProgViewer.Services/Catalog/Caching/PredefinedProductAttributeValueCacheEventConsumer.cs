using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a predefined product attribute value cache event consumer
    /// </summary>
    public partial class PredefinedProductAttributeValueCacheEventConsumer : CacheEventConsumer<PredefinedProductAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(PredefinedProductAttributeValue entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.PredefinedProductAttributeValuesByAttributeCacheKey, entity.ProductAttributeId);
        }
    }
}
