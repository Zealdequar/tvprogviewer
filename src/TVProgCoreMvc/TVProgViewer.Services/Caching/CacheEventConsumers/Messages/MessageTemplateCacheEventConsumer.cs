using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Messages
{
    /// <summary>
    /// Represents a message template cache event consumer
    /// </summary>
    public partial class MessageTemplateCacheEventConsumer : CacheEventConsumer<MessageTemplate>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(MessageTemplate entity)
        {
            RemoveByPrefix(TvProgMessageCachingDefaults.MessageTemplatesAllPrefixCacheKey);
            var prefix = TvProgMessageCachingDefaults.MessageTemplatesByNamePrefixCacheKey.ToCacheKey(entity.Name);
            RemoveByPrefix(prefix);

        }
    }
}
