using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Widgets.FacebookPixel.Models
{
    /// <summary>
    /// Represents a custom event model
    /// </summary>
    public record CustomEventModel : BaseTvProgModel
    {
        #region Ctor

        public CustomEventModel()
        {
            WidgetZonesIds = new List<int>();
            WidgetZones = new List<string>();
            AvailableWidgetZones = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ConfigurationId { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.EventName")]
        public string EventName { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.WidgetZones")]
        public IList<int> WidgetZonesIds { get; set; }
        public IList<string> WidgetZones { get; set; }
        public IList<SelectListItem> AvailableWidgetZones { get; set; }
        public string WidgetZonesName { get; set; }

        #endregion
    }
}