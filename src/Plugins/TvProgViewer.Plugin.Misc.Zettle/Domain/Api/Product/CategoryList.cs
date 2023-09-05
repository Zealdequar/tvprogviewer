using System.Collections.Generic;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.Product
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
        public List<Product.ProductCategory> Categories { get; set; }
    }
}