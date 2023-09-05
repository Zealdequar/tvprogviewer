using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a pickup point provider list model
    /// </summary>
    public partial record PickupPointProviderListModel : BasePagedListModel<PickupPointProviderModel>
    {
    }
}