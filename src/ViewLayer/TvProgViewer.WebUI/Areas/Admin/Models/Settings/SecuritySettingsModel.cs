using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a security settings model
    /// </summary>
    public partial record SecuritySettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
        public string EncryptionKey { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
        public string AdminAreaAllowedIpAddresses { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled")]
        public bool HoneypotEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ProgrammesLoadKey")]
        public string ProgrammesLoadKey { get; set; }

        #endregion
    }
}