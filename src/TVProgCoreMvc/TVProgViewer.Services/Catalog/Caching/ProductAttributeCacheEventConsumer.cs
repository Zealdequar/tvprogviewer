using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute cache event consumer
    /// </summary>
    public partial class ProductAttributeCacheEventConsumer : CacheEventConsumer<ProductAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override async Task ClearCacheAsync(ProductAttribute entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Insert)
                await RemoveAsync(TvProgCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
