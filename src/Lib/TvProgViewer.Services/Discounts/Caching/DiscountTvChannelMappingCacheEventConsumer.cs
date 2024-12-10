using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Discounts.Caching
{
    /// <summary>
    /// Represents a discount-tvchannel mapping cache event consumer
    /// </summary>
    public partial class DiscountTvChannelMappingCacheEventConsumer : CacheEventConsumer<DiscountTvChannelMapping>
    {
        protected override async Task ClearCacheAsync(DiscountTvChannelMapping entity)
        {
            await RemoveAsync(TvProgDiscountDefaults.AppliedDiscountsCacheKey, nameof(TvChannel), entity.EntityId);

            await base.ClearCacheAsync(entity);
        }
    }
}