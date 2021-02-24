using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record MultiFactorAuthenticationProviderModel : BaseTvProgEntityModel
    {
        public bool Selected { get; set; }

        public string Name { get; set; }

        public string SystemName { get; set; }

        public string LogoUrl { get; set; }

        public string Description { get; set; }

        public string ViewComponentName { get; set; }
    }
}
