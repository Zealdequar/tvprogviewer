using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a cross-sell tvChannel model to add to the tvChannel
    /// </summary>
    public partial record AddCrossSellTvChannelModel : BaseTvProgModel
    {
        #region Ctor

        public AddCrossSellTvChannelModel()
        {
            SelectedTvChannelIds = new List<int>();
        }
        #endregion

        #region Properties

        public int TvChannelId { get; set; }

        public IList<int> SelectedTvChannelIds { get; set; }

        #endregion
    }
}