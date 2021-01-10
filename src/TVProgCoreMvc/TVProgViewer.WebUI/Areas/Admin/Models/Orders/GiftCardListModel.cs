using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a gift card list model
    /// </summary>
    public record GiftCardListModel : BasePagedListModel<GiftCardModel>
    {
    }
}