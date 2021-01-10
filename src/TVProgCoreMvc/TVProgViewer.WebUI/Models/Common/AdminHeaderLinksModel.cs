using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record AdminHeaderLinksModel : BaseTvProgModel
    {
        public string ImpersonatedUserName { get; set; }
        public bool IsUserImpersonated { get; set; }
        public bool DisplayAdminLink { get; set; }
        public string EditPageUrl { get; set; }
    }
}