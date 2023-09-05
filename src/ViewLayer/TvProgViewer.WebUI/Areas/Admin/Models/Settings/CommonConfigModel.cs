using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a common configuration model
    /// </summary>
    public partial record CommonConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.DisplayFullErrorStack")]
        public bool DisplayFullErrorStack { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.UserAgentStringsPath")]
        public string UserAgentStringsPath { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.CrawlerOnlyUserAgentStringsPath")]
        public string CrawlerOnlyUserAgentStringsPath { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.UseSessionStateTempDataProvider")]
        public bool UseSessionStateTempDataProvider { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.MiniProfilerEnabled")]
        public bool MiniProfilerEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.ScheduleTaskRunTimeout")]
        [UIHint("Int32Nullable")]
        public int? ScheduleTaskRunTimeout { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.StaticFilesCacheControl")]
        public string StaticFilesCacheControl { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.PluginStaticFileExtensionsBlacklist")]
        public string PluginStaticFileExtensionsBlacklist { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.ServeUnknownFileTypes")]
        public bool ServeUnknownFileTypes { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Common.UseAutofac")]
        public bool UseAutofac { get; set; }

        #endregion
    }
}