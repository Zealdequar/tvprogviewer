using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record LogoModel : BaseTvProgModel
    {
        public string StoreName { get; set; }

        public string LogoPath { get; set; }
    }
}