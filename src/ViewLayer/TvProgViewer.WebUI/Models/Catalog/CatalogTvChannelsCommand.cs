using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Web.Framework.UI.Paging;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a model to get the catalog tvChannels
    /// </summary>
    public partial record CatalogTvChannelsCommand : BasePageableModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the price ('min-max' format)
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option ids
        /// </summary>
        [FromQuery(Name = "specs")]
        public List<int> SpecificationOptionIds { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer ids
        /// </summary>
        [FromQuery(Name = "ms")]
        public List<int> ManufacturerIds { get; set; }

        /// <summary>
        /// Gets or sets a order by
        /// </summary>
        public int? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a tvChannel sorting
        /// </summary>
        public string ViewMode { get; set; }

        #endregion
    }
}
