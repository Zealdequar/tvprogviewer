using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record UserAvatarModel : BaseTvProgModel
    {
        public string AvatarUrl { get; set; }
    }
}