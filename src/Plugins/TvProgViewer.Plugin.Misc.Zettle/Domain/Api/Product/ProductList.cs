using System.Collections.Generic;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents the tvChannels details
    /// </summary>
    public class TvChannelList : List<TvChannel>, IApiResponse
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error description
        /// </summary>
        [JsonProperty(PropertyName = "error_description")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the developer message
        /// </summary>
        [JsonProperty(PropertyName = "developerMessage")]
        public string DeveloperMessage { get; set; }
    }
}