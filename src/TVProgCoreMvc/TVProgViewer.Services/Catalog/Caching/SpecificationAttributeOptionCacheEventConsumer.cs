using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute option cache event consumer
    /// </summary>
    public partial class SpecificationAttributeOptionCacheEventConsumer : CacheEventConsumer<SpecificationAttributeOption>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override async Task ClearCacheAsync(SpecificationAttributeOption entity, EntityEventType entityEventType)
        {
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeOptionsCacheKey, entity.SpecificationAttributeId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);

            if (entityEventType == EntityEventType.Delete)
                await RemoveByPrefixAsync(TvProgCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
        }
    }
}
