using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount cache event consumer
    /// </summary>
    public partial class DiscountCacheEventConsumer : CacheEventConsumer<Discount>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(Discount entity)
        {
            await RemoveAsync(TvProgDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity);
            await RemoveByPrefixAsync(TvProgDiscountDefaults.CategoryIdsByDiscountPrefix, entity);
            await RemoveByPrefixAsync(TvProgDiscountDefaults.ManufacturerIdsByDiscountPrefix, entity);
            await RemoveByPrefixAsync(TvProgDiscountDefaults.AppliedDiscountsCachePrefix);
        }
    }
}
