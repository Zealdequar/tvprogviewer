using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a hosting configuration model
    /// </summary>
    public partial record HostingConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Hosting.UseProxy")]
        public bool UseProxy { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ForwardedForHeaderName")]
        public string ForwardedForHeaderName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ForwardedProtoHeaderName")]
        public string ForwardedProtoHeaderName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Hosting.KnownProxies")]
        public string KnownProxies { get; set; }

        #endregion
    }
}