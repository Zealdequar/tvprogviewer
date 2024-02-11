using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvchannel specification attribute cache event consumer
    /// </summary>
    public partial class TvChannelSpecificationAttributeCacheEventConsumer : CacheEventConsumer<TvChannelSpecificationAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(TvChannelSpecificationAttribute entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.TvChannelSpecificationAttributeByTvChannelPrefix, entity.TvChannelId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
            await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeGroupByTvChannelCacheKey, entity.TvChannelId);
        }
    }
}
