using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated tvChannel model to add to the tvChannel
    /// </summary>
    public partial record AddAssociatedTvChannelModel : BaseTvProgModel
    {
        #region Ctor

        public AddAssociatedTvChannelModel()
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