using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Misc.Zettle.Models
{
    /// <summary>
    /// Represents a tvChannel model to add for synchronization
    /// </summary>
    public record AddTvChannelToSyncModel : BaseTvProgModel
    {
        #region Ctor

        public AddTvChannelToSyncModel()
        {
            SelectedTvChannelIds = new List<int>();
        }

        #endregion

        #region Properties

        public IList<int> SelectedTvChannelIds { get; set; }

        #endregion
    }
}