﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.Image
{
    /// <summary>
    /// Represents request to get the image
    /// </summary>
    public class GetImageRequest : ImageApiRequest
    {
        /// <summary>
        /// Gets or sets the image lookup key
        /// </summary>
        [JsonIgnore]
        public string ImageLookupKey { get; set; }

        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => $"v2/images/organizations/self/tvChannel/{ImageLookupKey}";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}