using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount category list model
    /// </summary>
    public partial record DiscountCategoryListModel : BasePagedListModel<DiscountCategoryModel>
    {
    }
}