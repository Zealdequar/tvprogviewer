using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel attribute model
    /// </summary>
    public partial record TvChannelAttributeModel : BaseTvProgModel
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

        public TvChannelAttributeModel()
        {
            ValueIds = new List<int>();
        }

        #endregion
    }
}
