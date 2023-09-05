using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a queued email list model
    /// </summary>
    public partial record QueuedEmailListModel : BasePagedListModel<QueuedEmailModel>
    {
    }
}