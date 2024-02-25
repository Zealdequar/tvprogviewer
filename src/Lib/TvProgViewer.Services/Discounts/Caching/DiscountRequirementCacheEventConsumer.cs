using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount requirement cache event consumer
    /// </summary>
    public partial class DiscountRequirementCacheEventConsumer : CacheEventConsumer<DiscountRequirement>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(DiscountRequirement entity)
        {
            await RemoveAsync(TvProgDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity.DiscountId);
        }
    }
}
