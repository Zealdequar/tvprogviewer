using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Topics
{
    /// <summary>
    /// Represents a topic list model
    /// </summary>
    public partial record TopicListModel : BasePagedListModel<TopicModel>
    {
    }
}