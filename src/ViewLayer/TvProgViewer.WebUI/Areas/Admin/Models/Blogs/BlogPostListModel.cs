using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Blogs
{
    /// <summary>
    /// Represents a blog post list model
    /// </summary>
    public partial record BlogPostListModel : BasePagedListModel<BlogPostModel>
    {
    }
}