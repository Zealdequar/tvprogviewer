using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Polls
{
    /// <summary>
    /// Represents a poll answer list model
    /// </summary>
    public partial record PollAnswerListModel : BasePagedListModel<PollAnswerModel>
    {
    }
}