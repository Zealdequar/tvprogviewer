using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Profile
{
    public partial record ProfileIndexModel : BaseTvProgModel
    {
        public int UserProfileId { get; set; }
        public string ProfileTitle { get; set; }
        public int PostsPage { get; set; }
        public bool PagingPosts { get; set; }
        public bool ForumsEnabled { get; set; }
    }
}