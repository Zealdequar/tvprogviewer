using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute cache event consumer
    /// </summary>
    public partial class SpecificationAttributeCacheEventConsumer : CacheEventConsumer<SpecificationAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override async Task ClearCacheAsync(SpecificationAttribute entity, EntityEventType entityEventType)
        {
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);

            if (entityEventType != EntityEventType.Insert)
            {
                await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);
                await RemoveByPrefixAsync(TvProgCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
            }
        }
    }
}
