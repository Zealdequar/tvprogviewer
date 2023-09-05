using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models
{
    /// <summary>
    /// Represents verification model
    /// </summary>
    public record TokenModel : BaseTvProgModel
    {
        [TvProgResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.User.VerificationToken")]
        public string Token { get; set; }
    }
}
