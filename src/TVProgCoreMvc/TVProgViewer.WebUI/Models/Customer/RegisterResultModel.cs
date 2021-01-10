using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record RegisterResultModel : BaseTvProgModel
    {
        public string Result { get; set; }
    }
}