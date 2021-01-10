using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CacheEventConsumers;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a category template cache event consumer
    /// </summary>
    public partial class CategoryTemplateCacheEventConsumer : CacheEventConsumer<CategoryTemplate>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CategoryTemplate entity)
        {
            Remove(TvProgCatalogCachingDefaults.CategoryTemplatesAllCacheKey);
        }
    }
}
