using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-product mapping cache event consumer
    /// </summary>
    public partial class DiscountProductMappingCacheEventConsumer : CacheEventConsumer<DiscountProductMapping>
    {
        protected override async Task ClearCacheAsync(DiscountProductMapping entity)
        {
            await RemoveAsync(TvProgDiscountDefaults.AppliedDiscountsCacheKey, nameof(Product), entity.EntityId);

            await base.ClearCacheAsync(entity);
        }
    }
}