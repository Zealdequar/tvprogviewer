using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents an installation configuration model
    /// </summary>
    public partial record InstallationConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Installation.DisableSampleData")]
        public bool DisableSampleData { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Installation.DisabledPlugins")]
        public string DisabledPlugins { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Installation.InstallRegionalResources")]
        public bool InstallRegionalResources { get; set; }

        #endregion
    }
}