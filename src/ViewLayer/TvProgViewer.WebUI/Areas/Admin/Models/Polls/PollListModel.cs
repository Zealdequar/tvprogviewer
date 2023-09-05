using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Polls
{
    /// <summary>
    /// Represents a poll list model
    /// </summary>
    public partial record PollListModel : BasePagedListModel<PollModel>
    {
    }
}