using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record UserAvatarModel : BaseTvProgModel
    {
        public string AvatarUrl { get; set; }
    }
}