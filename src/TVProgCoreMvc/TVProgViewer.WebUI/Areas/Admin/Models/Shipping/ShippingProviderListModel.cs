using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a shipping provider list model
    /// </summary>
    public partial record ShippingProviderListModel : BasePagedListModel<ShippingProviderModel>
    {
    }
}