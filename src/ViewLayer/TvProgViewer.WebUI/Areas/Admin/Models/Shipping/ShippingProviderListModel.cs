using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a shipping provider list model
    /// </summary>
    public partial record ShippingProviderListModel : BasePagedListModel<ShippingProviderModel>
    {
    }
}