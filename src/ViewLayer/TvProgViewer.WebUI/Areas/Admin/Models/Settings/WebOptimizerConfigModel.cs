using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents WebOptimizer config model
    /// </summary>
    public partial record WebOptimizerConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableJavaScriptBundling")]
        public bool EnableJavaScriptBundling { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableCssBundling")]
        public bool EnableCssBundling { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableDiskCache")]
        public bool EnableDiskCache { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.CacheDirectory")]
        public string CacheDirectory { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.JavaScriptBundleSuffix")]
        public string JavaScriptBundleSuffix { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.CssBundleSuffix")]
        public string CssBundleSuffix { get; set; }

        #endregion
    }
}
