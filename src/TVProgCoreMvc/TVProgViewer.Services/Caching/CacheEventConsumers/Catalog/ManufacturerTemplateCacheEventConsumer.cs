using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a manufacturer template cache event consumer
    /// </summary>
    public partial class ManufacturerTemplateCacheEventConsumer : CacheEventConsumer<ManufacturerTemplate>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ManufacturerTemplate entity)
        {
            Remove(TvProgCatalogCachingDefaults.ManufacturerTemplatesAllCacheKey);
        }
    }
}
