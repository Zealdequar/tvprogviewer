using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.MultiFactorAuthentication
{
    /// <summary>
    /// Represents an multi-factor authentication method list model
    /// </summary>
    public partial record MultiFactorAuthenticationMethodListModel : BasePagedListModel<MultiFactorAuthenticationMethodModel>
    {
    }
}
