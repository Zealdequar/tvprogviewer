using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a related tvchannel model to add to the tvchannel
    /// </summary>
    public partial record AddRelatedTvChannelModel : BaseTvProgModel
    {
        #region Ctor

        public AddRelatedTvChannelModel()
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