using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a checkout attribute value list model
    /// </summary>
    public partial record CheckoutAttributeValueListModel : BasePagedListModel<CheckoutAttributeValueModel>
    {
    }
}