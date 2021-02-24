using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Discounts.Caching
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
        protected override async Task ClearCacheAsync(Discount entity)
        {
            await RemoveAsync(TvProgDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity);
            await RemoveByPrefixAsync(TvProgDiscountDefaults.CategoryIdsByDiscountPrefix, entity);
            await RemoveByPrefixAsync(TvProgDiscountDefaults.ManufacturerIdsByDiscountPrefix, entity);
        }
    }
}
