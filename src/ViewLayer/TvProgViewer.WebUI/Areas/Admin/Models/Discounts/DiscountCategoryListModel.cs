using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount category list model
    /// </summary>
    public partial record DiscountCategoryListModel : BasePagedListModel<DiscountCategoryModel>
    {
    }
}