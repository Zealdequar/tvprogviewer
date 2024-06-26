﻿using TvProgViewer.Core;

namespace TvProgViewer.Plugin.Widgets.AccessiBe
{
    /// <summary>
    /// Represents plugin default vaues and constants
    /// </summary>
    public class AccessiBeDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Widgets.AccessiBe";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"tvprogviewer-{TvProgVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Widgets.AccessiBe.Configure";
    }
}