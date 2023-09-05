using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute model
    /// </summary>
    public partial record ProductAttributeModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the attribute id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the value IDs of the attribute
        /// </summary>
        public IList<int> ValueIds { get; set; }

        #endregion

        #region Ctor

        public ProductAttributeModel()
        {
            ValueIds = new List<int>();
        }

        #endregion
    }
}
