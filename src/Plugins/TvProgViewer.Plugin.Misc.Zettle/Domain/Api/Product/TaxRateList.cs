﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents tax rates details
    /// </summary>
    public class TaxRateList : ApiResponse
    {
        /// <summary>
        /// Gets or sets a list of all tax rates
        /// </summary>
        [JsonProperty(PropertyName = "taxRates")]
        public List<TaxRate> TaxRates { get; set; }
    }
}