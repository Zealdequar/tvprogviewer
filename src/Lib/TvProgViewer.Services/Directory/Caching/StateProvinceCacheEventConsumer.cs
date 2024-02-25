﻿using System.Threading.Tasks;
﻿using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Directory.Caching
{
    /// <summary>
    /// Represents a state province cache event consumer
    /// </summary>
    public partial class StateProvinceCacheEventConsumer : CacheEventConsumer<StateProvince>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(StateProvince entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<StateProvince>.Prefix);
        }
    }
}
