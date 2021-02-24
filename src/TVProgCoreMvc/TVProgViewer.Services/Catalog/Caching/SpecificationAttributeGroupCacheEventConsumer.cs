using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute group cache event consumer
    /// </summary>
    public partial class SpecificationAttributeGroupCacheEventConsumer : CacheEventConsumer<SpecificationAttributeGroup>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override async Task ClearCacheAsync(SpecificationAttributeGroup entity, EntityEventType entityEventType)
        {
            if (entityEventType != EntityEventType.Insert)
                await RemoveByPrefixAsync(TvProgCatalogDefaults.SpecificationAttributeGroupByProductPrefix);
        }
    }
}
