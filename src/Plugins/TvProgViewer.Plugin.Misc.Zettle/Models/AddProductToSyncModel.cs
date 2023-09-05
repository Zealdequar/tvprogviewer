using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Misc.Zettle.Models
{
    /// <summary>
    /// Represents a product model to add for synchronization
    /// </summary>
    public record AddProductToSyncModel : BaseTvProgModel
    {
        #region Ctor

        public AddProductToSyncModel()
        {
            SelectedProductIds = new List<int>();
        }

        #endregion

        #region Properties

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}