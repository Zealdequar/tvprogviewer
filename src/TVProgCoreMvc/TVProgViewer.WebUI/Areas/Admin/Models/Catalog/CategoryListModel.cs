using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a category list model
    /// </summary>
    public partial record CategoryListModel : BasePagedListModel<CategoryModel>
    {
    }
}