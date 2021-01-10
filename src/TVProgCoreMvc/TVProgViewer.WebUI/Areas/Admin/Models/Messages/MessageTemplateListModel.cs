using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a message template list model
    /// </summary>
    public partial record MessageTemplateListModel : BasePagedListModel<MessageTemplateModel>
    {
    }
}