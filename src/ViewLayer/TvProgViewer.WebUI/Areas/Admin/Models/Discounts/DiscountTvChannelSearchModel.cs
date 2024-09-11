using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount tvChannel search model
    /// </summary>
    public partial record DiscountTvChannelSearchModel : BaseSearchModel
    {
        #region Properties

        public int DiscountId { get; set; }

        #endregion
    }
}