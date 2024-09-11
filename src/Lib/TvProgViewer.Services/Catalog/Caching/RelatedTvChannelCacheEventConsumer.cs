using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a related tvChannel cache event consumer
    /// </summary>
    public partial class RelatedTvChannelCacheEventConsumer : CacheEventConsumer<RelatedTvChannel>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(RelatedTvChannel entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.RelatedTvChannelsPrefix, entity.TvChannelId1);
        }
    }
}
