using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a cross-sell tvchannel search model
    /// </summary>
    public partial record CrossSellTvChannelSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        #endregion
    }
}