using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record GdprToolsModel : BaseTvProgModel
    {
        public string Result { get; set; }
    }
}