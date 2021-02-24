using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a hosting configuration model
    /// </summary>
    public partial record HostingConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Hosting.UseHttpClusterHttps")]
        public bool UseHttpClusterHttps { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Hosting.UseHttpXForwardedProto")]
        public bool UseHttpXForwardedProto { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ForwardedHttpHeader")]
        public string ForwardedHttpHeader { get; set; }

        #endregion
    }
}