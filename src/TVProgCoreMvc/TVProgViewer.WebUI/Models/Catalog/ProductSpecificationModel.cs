using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Specification attribute model
    /// </summary>
    public partial record ProductSpecificationModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the grouped specification attribute models
        /// </summary>
        public IList<ProductSpecificationAttributeGroupModel> Groups { get; set; }

        #endregion

        #region Ctor

        public ProductSpecificationModel()
        {
            Groups = new List<ProductSpecificationAttributeGroupModel>();
        }

        #endregion
    }
}