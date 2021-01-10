using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a queued email list model
    /// </summary>
    public partial record QueuedEmailListModel : BasePagedListModel<QueuedEmailModel>
    {
    }
}