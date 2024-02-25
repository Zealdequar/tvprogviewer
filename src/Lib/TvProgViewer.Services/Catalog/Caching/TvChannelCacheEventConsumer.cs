using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Caching;
using TvProgViewer.Services.Discounts;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvchannel cache event consumer
    /// </summary>
    public partial class TvChannelCacheEventConsumer : CacheEventConsumer<TvChannel>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannel entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelManufacturersByTvChannelPrefix, entity);
            await RemoveAsync(TvProgCatalogDefaults.TvChannelsHomepageCacheKey);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelPricePrefix, entity);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelMultiplePricePrefix, entity);
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.FeaturedTvChannelIdsPrefix);

            if (entityEventType == EntityEventType.Delete)
            {
                await RemoveByPrefixAsync(TvProgCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
                await RemoveByPrefixAsync(TvProgCatalogDefaults.ManufacturersByCategoryPrefix);
            }

            await RemoveAsync(TvProgDiscountDefaults.AppliedDiscountsCacheKey, nameof(TvChannel), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
