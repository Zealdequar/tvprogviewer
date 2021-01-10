using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Blogs
{
    /// <summary>
    /// Represents a blog post list model
    /// </summary>
    public partial record BlogPostListModel : BasePagedListModel<BlogPostModel>
    {
    }
}