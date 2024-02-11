using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated tvchannel search model
    /// </summary>
    public partial record AssociatedTvChannelSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        #endregion
    }
}