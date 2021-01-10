using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Topics
{
    /// <summary>
    /// Represents a topic list model
    /// </summary>
    public partial record TopicListModel : BasePagedListModel<TopicModel>
    {
    }
}