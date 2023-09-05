using System.Collections.Generic;
using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Domain.Cms
{
    /// <summary>
    /// Widget settings
    /// </summary>
    public partial class WidgetSettings : ISettings
    {
        public WidgetSettings()
        {
            ActiveWidgetSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets a system names of active widgets
        /// </summary>
        public List<string> ActiveWidgetSystemNames { get; set; }
    }
}