using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a shipping method list model
    /// </summary>
    public partial record ShippingMethodListModel : BasePagedListModel<ShippingMethodModel>
    {
    }
}