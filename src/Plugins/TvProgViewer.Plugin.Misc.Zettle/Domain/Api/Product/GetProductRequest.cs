using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to get the single tvChannel
    /// </summary>
    public class GetTvChannelRequest : TvChannelApiRequest, IConditionalRequest
    {
        /// <summary>
        /// Gets or sets the tvChannel unique identifier as UUID version 1
        /// </summary>
        [JsonIgnore]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the ETag header value
        /// </summary>
        [JsonIgnore]
        public string ETag { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/tvChannels/{Uuid}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}