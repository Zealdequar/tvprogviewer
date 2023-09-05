using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic template cache event consumer
    /// </summary>
    public partial class TopicTemplateCacheEventConsumer : CacheEventConsumer<TopicTemplate>
    {
    }
}
