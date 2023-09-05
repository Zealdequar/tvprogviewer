using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Profile
{
    public partial record PostsModel : BaseTvProgModel
    {
        public int ForumTopicId { get; set; }
        public string ForumTopicTitle { get; set; }
        public string ForumTopicSlug { get; set; }
        public string ForumPostText { get; set; }
        public string Posted { get; set; }
    }
}