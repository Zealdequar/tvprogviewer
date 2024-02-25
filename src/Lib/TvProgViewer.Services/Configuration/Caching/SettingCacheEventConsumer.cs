using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Configuration;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Configuration.Caching
{
    /// <summary>
    /// Represents a setting cache event consumer
    /// </summary>
    public partial class SettingCacheEventConsumer : CacheEventConsumer<Setting>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override Task ClearCacheAsync(Setting entity, EntityEventType entityEventType)
        {
            //clear setting cache in SettingService
            return Task.CompletedTask;
        }
    }
}