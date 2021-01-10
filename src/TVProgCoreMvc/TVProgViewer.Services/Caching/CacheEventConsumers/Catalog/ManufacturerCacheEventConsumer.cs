using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a manufacturer cache event consumer
    /// </summary>
    public partial class ManufacturerCacheEventConsumer : CacheEventConsumer<Manufacturer>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Manufacturer entity)
        {
            var prefix = TvProgCatalogCachingDefaults.ProductManufacturersByManufacturerPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(TvProgDiscountCachingDefaults.DiscountManufacturerIdsPrefixCacheKey);
        }
    }
}
