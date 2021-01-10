using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Blogs
{
    /// <summary>
    /// Represents a blog comment list model
    /// </summary>
    public partial record BlogCommentListModel : BasePagedListModel<BlogCommentModel>
    {
    }
}