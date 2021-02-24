using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Topics;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic cache event consumer
    /// </summary>
    public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Topic entity)
        {
            await RemoveByPrefixAsync(TvProgTopicDefaults.TopicBySystemNamePrefix, entity.SystemName);
        }
    }
}
