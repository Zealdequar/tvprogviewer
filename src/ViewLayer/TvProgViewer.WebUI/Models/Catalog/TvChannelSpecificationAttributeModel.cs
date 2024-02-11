using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute model
    /// </summary>
    public partial record TvChannelSpecificationAttributeModel : BaseTvProgEntityModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public IList<TvChannelSpecificationAttributeValueModel> Values { get; set; }

        #endregion

        #region Ctor

        public TvChannelSpecificationAttributeModel()
        {
            Values = new List<TvChannelSpecificationAttributeValueModel>();
        }

        #endregion
    }
}