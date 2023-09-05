using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;
using TvProgViewer.Services.Discounts;

namespace TvProgViewer.Services.Catalog.Caching
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
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Manufacturer entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(TvProgDiscountDefaults.ManufacturerIdsPrefix);

            if (entityEventType != EntityEventType.Insert)
                await RemoveByPrefixAsync(TvProgCatalogDefaults.ManufacturersByCategoryPrefix);

            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, entity);

            await RemoveAsync(TvProgDiscountDefaults.AppliedDiscountsCacheKey, nameof(Manufacturer), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
