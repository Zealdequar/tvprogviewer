using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a product combination model
    /// </summary>
    public partial record ProductCombinationModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        public IList<ProductAttributeModel> Attributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to the combination have stock
        /// </summary>
        public bool InStock { get; set; }

        #endregion

        #region Ctor

        public ProductCombinationModel()
        {
            Attributes = new List<ProductAttributeModel>();
        }

        #endregion
    }
}
