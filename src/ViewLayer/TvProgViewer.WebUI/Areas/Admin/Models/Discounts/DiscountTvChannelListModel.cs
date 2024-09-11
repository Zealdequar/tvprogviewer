using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount tvChannel list model
    /// </summary>
    public partial record DiscountTvChannelListModel : BasePagedListModel<DiscountTvChannelModel>
    {
    }
}