using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Blogs
{
    public partial record BlogPostTagModel : BaseTvProgModel
    {
        public string Name { get; set; }

        public int BlogPostCount { get; set; }
    }
}