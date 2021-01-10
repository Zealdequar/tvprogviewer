using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a news comment list model
    /// </summary>
    public partial record NewsCommentListModel : BasePagedListModel<NewsCommentModel>
    {
    }
}