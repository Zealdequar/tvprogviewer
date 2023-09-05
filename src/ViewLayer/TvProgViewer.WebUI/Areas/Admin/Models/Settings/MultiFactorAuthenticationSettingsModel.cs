using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a multi-factor authentication settings model
    /// </summary>
    public partial record MultiFactorAuthenticationSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.ForceMultifactorAuthentication")]
        public bool ForceMultifactorAuthentication { get; set; }

        #endregion
    }
}
