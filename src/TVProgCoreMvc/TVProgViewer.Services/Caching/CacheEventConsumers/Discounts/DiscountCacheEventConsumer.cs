using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Discounts
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
        protected override void ClearCache(Discount entity)
        {
            RemoveByPrefix(TvProgDiscountCachingDefaults.DiscountAllPrefixCacheKey);
            var cacheKey = TvProgDiscountCachingDefaults.DiscountRequirementModelCacheKey.FillCacheKey(entity);
            Remove(cacheKey);

            var prefix = TvProgDiscountCachingDefaults.DiscountCategoryIdsByDiscountPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            prefix = TvProgDiscountCachingDefaults.DiscountManufacturerIdsByDiscountPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

        }
    }
}
