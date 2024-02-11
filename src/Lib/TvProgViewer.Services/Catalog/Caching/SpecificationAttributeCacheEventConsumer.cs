using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
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
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(SpecificationAttribute entity, EntityEventType entityEventType)
        {
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);

            if (entityEventType != EntityEventType.Insert)
            {
                await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelSpecificationAttributeAllByTvChannelPrefix);
                await RemoveByPrefixAsync(TvProgCatalogDefaults.SpecificationAttributeGroupByTvChannelPrefix);
                await RemoveByPrefixAsync(TvProgCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
            }

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
