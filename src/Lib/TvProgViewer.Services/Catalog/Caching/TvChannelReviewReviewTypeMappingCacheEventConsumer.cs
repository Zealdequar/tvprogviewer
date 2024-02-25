using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvchannel review review type cache event consumer
    /// </summary>
    public partial class TvChannelReviewReviewTypeMappingCacheEventConsumer : CacheEventConsumer<TvChannelReviewReviewTypeMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannelReviewReviewTypeMapping entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.TvChannelReviewTypeMappingByReviewTypeCacheKey, entity.TvChannelReviewId);
        }
    }
}
