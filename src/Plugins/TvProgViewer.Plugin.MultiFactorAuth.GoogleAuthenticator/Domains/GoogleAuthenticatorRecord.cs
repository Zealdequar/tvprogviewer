using TvProgViewer.Core;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains
{
    /// <summary>
    /// Represents a  Google Authenticator configuration
    /// </summary>
    public class GoogleAuthenticatorRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets a user identifier
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets a SecretKey identifier
        /// </summary>
        public string SecretKey { get; set; }
    }
}
