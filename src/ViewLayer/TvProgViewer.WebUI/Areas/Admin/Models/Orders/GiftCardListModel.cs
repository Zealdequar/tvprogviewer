using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a gift card list model
    /// </summary>
    public partial record GiftCardListModel : BasePagedListModel<GiftCardModel>
    {
    }
}