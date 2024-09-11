using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to create import of multiple tvChannel
    /// </summary>
    public class CreateImportRequest : TvChannelApiRequest
    {
        /// <summary>
        /// Gets or sets the tvChannels
        /// </summary>
        [JsonProperty(PropertyName = "tvChannels")]
        public List<TvChannel> TvChannels { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public override string Path => "organizations/self/import/v2";

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}