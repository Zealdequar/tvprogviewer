using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Newsletter
{
    public partial record SubscriptionActivationModel : BaseTvProgModel
    {
        public string Result { get; set; }
    }
}