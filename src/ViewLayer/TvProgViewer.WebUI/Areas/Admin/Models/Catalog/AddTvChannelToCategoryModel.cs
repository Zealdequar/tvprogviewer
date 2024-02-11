using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel model to add to the category
    /// </summary>
    public partial record AddTvChannelToCategoryModel : BaseTvProgModel
    {
        #region Ctor

        public AddTvChannelToCategoryModel()
        {
            SelectedTvChannelIds = new List<int>();
        }
        #endregion

        #region Properties

        public int CategoryId { get; set; }

        public IList<int> SelectedTvChannelIds { get; set; }

        #endregion
    }
}