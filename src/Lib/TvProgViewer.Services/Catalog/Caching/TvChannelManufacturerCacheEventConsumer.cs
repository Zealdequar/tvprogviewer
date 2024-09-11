using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvChannel manufacturer cache event consumer
    /// </summary>
    public partial class TvChannelManufacturerCacheEventConsumer : CacheEventConsumer<TvChannelManufacturer>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannelManufacturer entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelManufacturersByTvChannelPrefix, entity.TvChannelId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelPricePrefix, entity.TvChannelId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelMultiplePricePrefix, entity.TvChannelId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ManufacturerFeaturedTvChannelIdsPrefix, entity.ManufacturerId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ManufacturersByCategoryPrefix);
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, entity.ManufacturerId.ToString());
        }
    }
}
