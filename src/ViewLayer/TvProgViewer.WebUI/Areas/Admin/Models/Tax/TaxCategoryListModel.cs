using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Tax
{
    /// <summary>
    /// Represents a tax category list model
    /// </summary>
    public partial record TaxCategoryListModel : BasePagedListModel<TaxCategoryModel>
    {
    }
}