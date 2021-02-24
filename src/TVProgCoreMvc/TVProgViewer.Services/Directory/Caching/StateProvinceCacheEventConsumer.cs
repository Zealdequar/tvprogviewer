﻿using System.Threading.Tasks;
﻿using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Directory.Caching
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
        protected override async Task ClearCacheAsync(StateProvince entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(TvProgEntityCacheDefaults<StateProvince>.Prefix);
        }
    }
}
