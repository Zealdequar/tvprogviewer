using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.ExternalAuthentication
{
    /// <summary>
    /// Represents an external authentication method list model
    /// </summary>
    public partial record ExternalAuthenticationMethodListModel : BasePagedListModel<ExternalAuthenticationMethodModel>
    {
    }
}