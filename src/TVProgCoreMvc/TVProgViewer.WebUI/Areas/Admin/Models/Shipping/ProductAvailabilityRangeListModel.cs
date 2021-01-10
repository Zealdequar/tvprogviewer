using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a product availability range list model
    /// </summary>
    public partial record ProductAvailabilityRangeListModel : BasePagedListModel<ProductAvailabilityRangeModel>
    {
    }
}