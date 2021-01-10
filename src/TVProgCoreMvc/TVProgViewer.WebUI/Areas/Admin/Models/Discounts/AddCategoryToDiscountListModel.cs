using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a category list model to add to the discount
    /// </summary>
    public partial record AddCategoryToDiscountListModel : BasePagedListModel<CategoryModel>
    {
    }
}