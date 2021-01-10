using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
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
        protected override void ClearCache(SpecificationAttributeOption entity)
        {
            Remove(TvProgCatalogCachingDefaults.SpecAttributesWithOptionsCacheKey);
            Remove(TvProgCatalogCachingDefaults.SpecAttributesOptionsCacheKey.FillCacheKey(entity.SpecificationAttributeId));
        }
    }
}
