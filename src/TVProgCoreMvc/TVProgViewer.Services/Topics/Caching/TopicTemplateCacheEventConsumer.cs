using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Topics;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic template cache event consumer
    /// </summary>
    public partial class TopicTemplateCacheEventConsumer : CacheEventConsumer<TopicTemplate>
    {
    }
}
