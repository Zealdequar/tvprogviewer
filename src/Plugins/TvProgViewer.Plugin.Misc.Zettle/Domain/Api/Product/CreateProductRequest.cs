using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to create the single tvChannel
    /// </summary>
    public class CreateTvChannelRequest : TvChannel, IApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets the request base URL
        /// </summary>
        [JsonIgnore]
        public string BaseUrl => "https://tvChannels.izettle.com/";

        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public string Path => "organizations/self/tvChannels";

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public string Method => HttpMethods.Post;
    }
}