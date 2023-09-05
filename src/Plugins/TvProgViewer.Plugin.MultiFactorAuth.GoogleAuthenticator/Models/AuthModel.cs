using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models
{
    /// <summary>
    /// Represents authentication model
    /// </summary>
    public record AuthModel : BaseTvProgModel
    {
        public string SecretKey { get; set; }

        [TvProgResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.User.VerificationToken")]
        public string Code { get; set; }

        public string QrCodeImageUrl { get; set; }

        [TvProgResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.User.ManualSetupCode")]
        public string ManualEntryQrCode { get; set; }

        public string Account { get; set; }
    }
}
