using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount tvchannel model
    /// </summary>
    public partial record DiscountTvChannelModel : BaseTvProgEntityModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        public string TvChannelName { get; set; }

        #endregion
    }
}