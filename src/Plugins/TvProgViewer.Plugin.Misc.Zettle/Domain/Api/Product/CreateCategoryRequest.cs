﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to create category
    /// </summary>
    public class CreateCategoryRequest : TvChannelApiRequest
    {
        /// <summary>
        /// Gets or sets the categories
        /// </summary>
        [JsonProperty(PropertyName = "categories")]
        public List<TvChannel.TvChannelCategory> Categories { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        [JsonIgnore]
        public override string Path => "organizations/self/categories/v2";

        /// <summary>
        /// Gets the request method
        /// </summary>
        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}