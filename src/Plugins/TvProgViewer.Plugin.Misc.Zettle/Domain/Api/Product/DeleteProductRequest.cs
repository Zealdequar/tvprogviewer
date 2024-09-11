using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to delete the single tvChannel
    /// </summary>
    public class DeleteTvChannelRequest : TvChannelApiRequest
    {
        /// <summary>
        /// Gets or sets the tvChannel unique identifier as UUID version 1
        /// </summary>
        [JsonIgnore]
        public string TvChannelUuid { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/tvChannels/{TvChannelUuid}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Delete;
    }
}