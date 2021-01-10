using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a delivery date list model
    /// </summary>
    public partial record DeliveryDateListModel : BasePagedListModel<DeliveryDateModel>
    {
    }
}