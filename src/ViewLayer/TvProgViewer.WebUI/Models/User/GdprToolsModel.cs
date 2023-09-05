using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record GdprToolsModel : BaseTvProgModel
    {
        public string Result { get; set; }
    }
}