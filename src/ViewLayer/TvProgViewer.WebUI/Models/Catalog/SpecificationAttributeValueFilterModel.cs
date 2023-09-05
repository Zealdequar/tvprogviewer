using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute value filter model
    /// </summary>
    public partial record SpecificationAttributeValueFilterModel : BaseTvProgEntityModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the specification attribute option name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option color (RGB)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the value is selected
        /// </summary>
        public bool Selected { get; set; }

        #endregion
    }
}
