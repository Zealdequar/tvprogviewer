using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a checkout attribute value list model
    /// </summary>
    public record CheckoutAttributeValueListModel : BasePagedListModel<CheckoutAttributeValueModel>
    {
    }
}