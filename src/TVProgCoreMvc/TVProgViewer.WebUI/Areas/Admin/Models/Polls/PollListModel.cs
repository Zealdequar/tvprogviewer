using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Polls
{
    /// <summary>
    /// Represents a poll list model
    /// </summary>
    public partial record PollListModel : BasePagedListModel<PollModel>
    {
    }
}