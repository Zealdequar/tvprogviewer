using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel specification model
    /// </summary>
    public partial record TvChannelSpecificationModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the grouped specification attribute models
        /// </summary>
        public IList<TvChannelSpecificationAttributeGroupModel> Groups { get; set; }

        #endregion

        #region Ctor

        public TvChannelSpecificationModel()
        {
            Groups = new List<TvChannelSpecificationAttributeGroupModel>();
        }

        #endregion
    }
}