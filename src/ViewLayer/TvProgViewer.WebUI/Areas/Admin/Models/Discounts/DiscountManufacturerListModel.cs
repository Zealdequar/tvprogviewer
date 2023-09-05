using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount manufacturer list model
    /// </summary>
    public partial record DiscountManufacturerListModel : BasePagedListModel<DiscountManufacturerModel>
    {
    }
}