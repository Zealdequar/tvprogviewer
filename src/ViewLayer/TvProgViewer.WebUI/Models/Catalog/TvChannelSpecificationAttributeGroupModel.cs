using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a grouped tvChannel specification attribute model
    /// </summary>
    public partial record TvChannelSpecificationAttributeGroupModel : BaseTvProgEntityModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the specification attribute group name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute group attributes
        /// </summary>
        public IList<TvChannelSpecificationAttributeModel> Attributes { get; set; }

        #endregion

        #region Ctor

        public TvChannelSpecificationAttributeGroupModel()
        {
            Attributes = new List<TvChannelSpecificationAttributeModel>();
        }

        #endregion
    }
}