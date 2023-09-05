using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Widgets.What3words.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>
    public record ConfigurationModel : BaseTvProgModel
    {
        [TvProgResourceDisplayName("Plugins.Widgets.What3words.Configuration.Fields.Enabled")]
        public bool Enabled { get; set; }
    }
}