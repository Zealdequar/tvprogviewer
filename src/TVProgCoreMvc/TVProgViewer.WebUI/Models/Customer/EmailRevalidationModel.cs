using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record EmailRevalidationModel : BaseTvProgModel
    {
        public string Result { get; set; }
    }
}