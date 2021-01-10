using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record AccountActivationModel : BaseTvProgModel
    {
        public string Result { get; set; }
    }
}