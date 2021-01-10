using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Discounts
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
        protected override void ClearCache(DiscountRequirement entity)
        {
            var cacheKey = TvProgDiscountCachingDefaults.DiscountRequirementModelCacheKey.FillCacheKey(entity.DiscountId);
            Remove(cacheKey);
        }
    }
}
