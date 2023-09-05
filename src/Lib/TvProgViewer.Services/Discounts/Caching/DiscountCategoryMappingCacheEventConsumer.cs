using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-category mapping cache event consumer
    /// </summary>
    public partial class DiscountCategoryMappingCacheEventConsumer : CacheEventConsumer<DiscountCategoryMapping>
    {
        protected override async Task ClearCacheAsync(DiscountCategoryMapping entity)
        {
            await RemoveAsync(TvProgDiscountDefaults.AppliedDiscountsCacheKey, nameof(Category), entity.EntityId);
            
            await base.ClearCacheAsync(entity);
        }
    }
}