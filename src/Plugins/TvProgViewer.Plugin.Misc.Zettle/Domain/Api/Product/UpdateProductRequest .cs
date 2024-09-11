using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to update the tvChannel
    /// </summary>
    public class UpdateTvChannelRequest : TvChannel, IApiRequest, IAuthorizedRequest, IConditionalRequest
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
        public string Path => $"organizations/self/tvChannels/v2/{Uuid}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public string Method => HttpMethods.Put;
    }
}