using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel specification attribute value model
    /// </summary>
    public partial record TvChannelSpecificationAttributeValueModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the attribute type id
        /// </summary>
        public int AttributeTypeId { get; set; }

        /// <summary>
        /// Gets or sets the value raw. This value is already HTML encoded
        /// </summary>
        public string ValueRaw { get; set; }

        /// <summary>
        /// Gets or sets the option color (if specified). Used to display color squares
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        #endregion
    }
}
