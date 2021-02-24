using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Configuration;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Configuration.Caching
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
        protected override Task ClearCacheAsync(Setting entity, EntityEventType entityEventType)
        {
            //clear setting cache in SettingService
            return Task.CompletedTask;
        }
    }
}