using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Tax
{
    /// <summary>
    /// Represents a tax category list model
    /// </summary>
    public partial record TaxCategoryListModel : BasePagedListModel<TaxCategoryModel>
    {
    }
}