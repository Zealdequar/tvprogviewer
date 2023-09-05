using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(SpecificationAttributeOption entity, EntityEventType entityEventType)
        {
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeOptionsCacheKey, entity.SpecificationAttributeId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);

            if (entityEventType == EntityEventType.Delete)
                await RemoveByPrefixAsync(TvProgCatalogDefaults.SpecificationAttributeGroupByProductPrefix);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
