using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a shipping method list model
    /// </summary>
    public partial record ShippingMethodListModel : BasePagedListModel<ShippingMethodModel>
    {
    }
}