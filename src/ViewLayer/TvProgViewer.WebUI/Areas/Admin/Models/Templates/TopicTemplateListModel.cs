using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a topic template list model
    /// </summary>
    public partial record TopicTemplateListModel : BasePagedListModel<TopicTemplateModel>
    {
    }
}