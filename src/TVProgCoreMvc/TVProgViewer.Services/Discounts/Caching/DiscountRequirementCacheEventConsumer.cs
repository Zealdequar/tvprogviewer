using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Discounts.Caching
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
        protected override async Task ClearCacheAsync(DiscountRequirement entity)
        {
            await RemoveAsync(TvProgDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity.DiscountId);
        }
    }
}
