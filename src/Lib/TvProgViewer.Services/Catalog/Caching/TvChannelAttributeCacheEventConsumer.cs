﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvChannel attribute cache event consumer
    /// </summary>
    public partial class TvChannelAttributeCacheEventConsumer : CacheEventConsumer<TvChannelAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(TvChannelAttribute entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Insert)
                await RemoveAsync(TvProgCatalogDefaults.TvChannelAttributeValuesByAttributeCacheKey, entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
