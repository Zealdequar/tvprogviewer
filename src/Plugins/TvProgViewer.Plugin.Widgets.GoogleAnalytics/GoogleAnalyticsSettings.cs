﻿using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Plugin.Widgets.GoogleAnalytics
{
    public class GoogleAnalyticsSettings : ISettings
    {
        public string GoogleId { get; set; }
        public string TrackingScript { get; set; }
        public bool EnableEcommerce { get; set; }
        public bool UseJsToSendEcommerceInfo { get; set; }
        public bool IncludingTax { get; set; }
        public string WidgetZone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include user identifier to script
        /// </summary>
        public bool IncludeUserId { get; set; }
    }
}