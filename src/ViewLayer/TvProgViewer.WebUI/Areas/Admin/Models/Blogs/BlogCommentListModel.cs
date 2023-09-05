using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Blogs
{
    /// <summary>
    /// Represents a blog comment list model
    /// </summary>
    public partial record BlogCommentListModel : BasePagedListModel<BlogCommentModel>
    {
    }
}