using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Widgets.FacebookPixel.Models
{
    /// <summary>
    /// Represents a custom event search model
    /// </summary>
    public record CustomEventSearchModel : BaseSearchModel
    {
        #region Ctor

        public CustomEventSearchModel()
        {
            AddCustomEvent = new CustomEventModel();
        }

        #endregion

        #region Properties

        public int ConfigurationId { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Search.WidgetZone")]
        public string WidgetZone { get; set; }

        public CustomEventModel AddCustomEvent { get; set; }

        #endregion
    }
}