using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a checkout attribute list model
    /// </summary>
    public record CheckoutAttributeListModel : BasePagedListModel<CheckoutAttributeModel>
    {
    }
}