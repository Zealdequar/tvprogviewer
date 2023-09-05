using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a checkout attribute list model
    /// </summary>
    public partial record CheckoutAttributeListModel : BasePagedListModel<CheckoutAttributeModel>
    {
    }
}