using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount tvchannel list model
    /// </summary>
    public partial record DiscountTvChannelListModel : BasePagedListModel<DiscountTvChannelModel>
    {
    }
}