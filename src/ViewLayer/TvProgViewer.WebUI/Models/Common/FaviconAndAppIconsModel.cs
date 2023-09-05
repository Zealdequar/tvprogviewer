using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record FaviconAndAppIconsModel : BaseTvProgModel
    {
        public string HeadCode { get; set; }
    }
}