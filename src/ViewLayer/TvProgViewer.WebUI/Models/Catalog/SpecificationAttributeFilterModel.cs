using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute filter model
    /// </summary>
    public partial record SpecificationAttributeFilterModel : BaseTvProgEntityModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the specification attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public IList<SpecificationAttributeValueFilterModel> Values { get; set; }

        #endregion

        #region Ctor

        public SpecificationAttributeFilterModel()
        {
            Values = new List<SpecificationAttributeValueFilterModel>();
        }

        #endregion
    }
}
