using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Topics.Caching
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override async Task ClearCacheAsync(Topic entity)
        {
            await RemoveByPrefixAsync(TvProgTopicDefaults.TopicBySystemNamePrefix, entity.SystemName);
        }
    }
}
