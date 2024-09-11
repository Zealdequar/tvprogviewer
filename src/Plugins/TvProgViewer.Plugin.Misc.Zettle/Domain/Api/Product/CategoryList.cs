using System.Collections.Generic;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents categories details
    /// </summary>
    public class CategoryList : ApiResponse
    {
        /// <summary>
        /// Gets or sets a list of all categories
        /// </summary>
        [JsonProperty(PropertyName = "categories")]
        public List<TvChannel.TvChannelCategory> Categories { get; set; }
    }
}