using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record EmailRevalidationModel : BaseTvProgModel
    {
        public string Result { get; set; }

        public string ReturnUrl { get; set; }
    }
}