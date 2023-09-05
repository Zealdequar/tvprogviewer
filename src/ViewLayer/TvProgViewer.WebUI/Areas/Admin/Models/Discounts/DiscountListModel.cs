using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount list model
    /// </summary>
    public partial record DiscountListModel : BasePagedListModel<DiscountModel>
    {
    }
}