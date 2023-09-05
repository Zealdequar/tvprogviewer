using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models
{
    /// <summary>
    /// Represents GoogleAuthenticator model
    /// </summary>
    public record GoogleAuthenticatorModel : BaseTvProgEntityModel
    {
        public string User { get; set; }

        public string SecretKey { get; set; }
    }
}
