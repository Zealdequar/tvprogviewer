using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to get all tvChannels
    /// </summary>
    public class GetTvChannelsRequest : TvChannelApiRequest
    {
        /// <summary>
        /// Gets or sets a value indicating whether to sorts tvChannels by created date
        /// </summary>
        [JsonIgnore]
        public bool SortByDate { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"organizations/self/tvChannels/v2?sort={SortByDate.ToString().ToLower()}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}