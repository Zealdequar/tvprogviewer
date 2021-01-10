using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount product list model
    /// </summary>
    public partial record DiscountProductListModel : BasePagedListModel<DiscountProductModel>
    {
    }
}