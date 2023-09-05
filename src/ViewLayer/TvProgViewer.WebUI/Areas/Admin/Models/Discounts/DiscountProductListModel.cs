using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount product list model
    /// </summary>
    public partial record DiscountProductListModel : BasePagedListModel<DiscountProductModel>
    {
    }
}