using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record LogoModel : BaseTvProgModel
    {
        public string StoreName { get; set; }

        public string LogoPath { get; set; }
    }
}