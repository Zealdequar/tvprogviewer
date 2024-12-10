using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvchannel-tvchannel tag mapping  cache event consumer
    /// </summary>
    public partial class TvChannelTvChannelTagMappingCacheEventConsumer : CacheEventConsumer<TvChannelTvChannelTagMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannelTvChannelTagMapping entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.TvChannelTagsByTvChannelCacheKey, entity.TvChannelId);
        }
    }
}