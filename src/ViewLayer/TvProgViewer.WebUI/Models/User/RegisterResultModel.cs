using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record RegisterResultModel : BaseTvProgModel
    {
        public string Result { get; set; }

        public string ReturnUrl { get; set; }
    }
}