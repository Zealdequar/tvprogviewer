using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models
{
    /// <summary>
    /// Represents GoogleAuthenticator search model
    /// </summary>
    public record GoogleAuthenticatorSearchModel : BaseSearchModel
    {
        [TvProgResourceDisplayName("Admin.Users.Users.List.SearchEmail")]
        public string SearchEmail { get; set; }

        public bool HideSearchBlock { get; set; }
    }
}
