﻿using System.Threading.Tasks;
﻿using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvchannel tag cache event consumer
    /// </summary>
    public partial class TvChannelTagCacheEventConsumer : CacheEventConsumer<TvChannelTag>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannelTag entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<TvChannelTag>.Prefix);
        }
    }
}
