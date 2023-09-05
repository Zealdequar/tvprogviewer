using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Widgets.AccessiBe.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseTvProgModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.Enabled")]
        public bool Enabled { get; set; }
        public bool Enabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.Script")]
        public string Script { get; set; }
        public bool Script_OverrideForStore { get; set; }

        public string Url { get; set; }

        #endregion
    }
}