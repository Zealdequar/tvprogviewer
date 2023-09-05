using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Caching;
using TvProgViewer.Services.Discounts;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product cache event consumer
    /// </summary>
    public partial class ProductCacheEventConsumer : CacheEventConsumer<Product>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Product entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductManufacturersByProductPrefix, entity);
            await RemoveAsync(TvProgCatalogDefaults.ProductsHomepageCacheKey);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductPricePrefix, entity);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.ProductMultiplePricePrefix, entity);
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.FeaturedProductIdsPrefix);

            if (entityEventType == EntityEventType.Delete)
            {
                await RemoveByPrefixAsync(TvProgCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
                await RemoveByPrefixAsync(TvProgCatalogDefaults.ManufacturersByCategoryPrefix);
            }

            await RemoveAsync(TvProgDiscountDefaults.AppliedDiscountsCacheKey, nameof(Product), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
