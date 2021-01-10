using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount list model
    /// </summary>
    public partial record DiscountListModel : BasePagedListModel<DiscountModel>
    {
    }
}