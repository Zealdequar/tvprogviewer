using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.ExternalAuthentication
{
    /// <summary>
    /// Represents an external authentication method list model
    /// </summary>
    public partial record ExternalAuthenticationMethodListModel : BasePagedListModel<ExternalAuthenticationMethodModel>
    {
    }
}