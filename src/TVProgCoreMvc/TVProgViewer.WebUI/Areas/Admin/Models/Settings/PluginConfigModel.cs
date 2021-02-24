using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a plugin configuration model
    /// </summary>
    public partial record PluginConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup")]
        public bool ClearPluginShadowDirectoryOnStartup { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup")]
        public bool CopyLockedPluginAssembilesToSubdirectoriesOnStartup { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Plugin.UseUnsafeLoadAssembly")]
        public bool UseUnsafeLoadAssembly { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy")]
        public bool UsePluginsShadowCopy { get; set; }

        #endregion
    }
}