using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a specification filter model
    /// </summary>
    public partial record SpecificationFilterModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the filtrable specification attributes
        /// </summary>
        public IList<SpecificationAttributeFilterModel> Attributes { get; set; }

        #endregion

        #region Ctor

        public SpecificationFilterModel()
        {
            Attributes = new List<SpecificationAttributeFilterModel>();
        }

        #endregion
    }
}
