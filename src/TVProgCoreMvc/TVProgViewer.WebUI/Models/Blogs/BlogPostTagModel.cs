using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Blogs
{
    public partial record BlogPostTagModel : BaseTvProgModel
    {
        public string Name { get; set; }

        public int BlogPostCount { get; set; }
    }
}