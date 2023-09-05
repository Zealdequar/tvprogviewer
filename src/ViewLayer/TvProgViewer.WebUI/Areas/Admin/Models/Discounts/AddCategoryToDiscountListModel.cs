using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a category list model to add to the discount
    /// </summary>
    public partial record AddCategoryToDiscountListModel : BasePagedListModel<CategoryModel>
    {
    }
}