using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a news comment list model
    /// </summary>
    public partial record NewsCommentListModel : BasePagedListModel<NewsCommentModel>
    {
    }
}