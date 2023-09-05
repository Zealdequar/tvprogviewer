using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>
    public record ConfigurationModel : BaseTvProgModel 
    {
        #region Ctor

        public ConfigurationModel()
        {
            GoogleAuthenticatorSearchModel = new GoogleAuthenticatorSearchModel();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.QRPixelsPerModule")]
        public int QRPixelsPerModule { get; set; }

        [TvProgResourceDisplayName("Plugins.MultiFactorAuth.GoogleAuthenticator.BusinessPrefix")]
        public string BusinessPrefix { get; set; }

        public GoogleAuthenticatorSearchModel GoogleAuthenticatorSearchModel { get; set; }

        #endregion
    }
}
