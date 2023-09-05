using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a message template list model
    /// </summary>
    public partial record MessageTemplateListModel : BasePagedListModel<MessageTemplateModel>
    {
    }
}