using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvchannel category cache event consumer
    /// </summary>
    public partial class TvChannelCategoryCacheEventConsumer : CacheEventConsumer<TvChannelCategory>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannelCategory entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelCategoriesByTvChannelPrefix, entity.TvChannelId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoryTvChannelsNumberPrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelPricePrefix, entity.TvChannelId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelMultiplePricePrefix, entity.TvChannelId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoryFeaturedTvChannelsIdsPrefix, entity.CategoryId);
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, entity.CategoryId.ToString());
            await RemoveAsync(TvProgCatalogDefaults.ManufacturersByCategoryCacheKey, entity.CategoryId.ToString());
        }
    }
}
