﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a specification attribute group cache event consumer
    /// </summary>
    public partial class SpecificationAttributeGroupCacheEventConsumer : CacheEventConsumer<SpecificationAttributeGroup>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(SpecificationAttributeGroup entity, EntityEventType entityEventType)
        {
            if (entityEventType != EntityEventType.Insert)
                await RemoveByPrefixAsync(TvProgCatalogDefaults.SpecificationAttributeGroupByTvChannelPrefix);
        }
    }
}
