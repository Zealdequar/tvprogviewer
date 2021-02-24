using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
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
