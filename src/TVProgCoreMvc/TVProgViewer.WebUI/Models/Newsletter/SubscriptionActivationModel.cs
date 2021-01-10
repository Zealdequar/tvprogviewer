using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Newsletter
{
    public partial record SubscriptionActivationModel : BaseTvProgModel
    {
        public string Result { get; set; }
    }
}