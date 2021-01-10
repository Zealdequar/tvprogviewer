using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount manufacturer list model
    /// </summary>
    public partial record DiscountManufacturerListModel : BasePagedListModel<DiscountManufacturerModel>
    {
    }
}