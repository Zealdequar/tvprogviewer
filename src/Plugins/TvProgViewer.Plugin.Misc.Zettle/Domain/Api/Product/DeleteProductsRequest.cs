using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to delete multiple tvChannels
    /// </summary>
    public class DeleteTvChannelsRequest : TvChannelApiRequest
    {
        /// <summary>
        /// Gets or sets the list of tvChannel unique identifier as UUID version 1
        /// </summary>
        [JsonIgnore]
        public List<string> TvChannelUuids { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/tvChannels?uuid={string.Join("&uuid=", TvChannelUuids)}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Delete;
    }
}