using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-manufacturer mapping cache event consumer
    /// </summary>
    public partial class DiscountManufacturerMappingCacheEventConsumer : CacheEventConsumer<DiscountManufacturerMapping>
    {
        protected override async Task ClearCacheAsync(DiscountManufacturerMapping entity)
        {
            await RemoveAsync(TvProgDiscountDefaults.AppliedDiscountsCacheKey, nameof(Manufacturer), entity.EntityId);

            await base.ClearCacheAsync(entity);
        }
    }
}