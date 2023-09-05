using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models
{
    /// <summary>
    /// Represents GoogleAuthenticator list model
    /// </summary>
    public record GoogleAuthenticatorListModel : BasePagedListModel<GoogleAuthenticatorModel>
    {
    }
}
