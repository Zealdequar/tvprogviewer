﻿using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Sendinblue.MarketingAutomation
{
    /// <summary>
    /// Represents base request to service
    /// </summary>
    public abstract class Request
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public abstract string Path { get; }

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public abstract string Method { get; }
    }
}