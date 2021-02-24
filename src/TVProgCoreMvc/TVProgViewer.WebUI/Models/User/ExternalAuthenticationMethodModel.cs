using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record ExternalAuthenticationMethodModel : BaseTvProgModel
    {
        public string ViewComponentName { get; set; }
    }
}