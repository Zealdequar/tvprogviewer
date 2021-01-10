using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record FaviconAndAppIconsModel : BaseTvProgModel
    {
        public string HeadCode { get; set; }
    }
}