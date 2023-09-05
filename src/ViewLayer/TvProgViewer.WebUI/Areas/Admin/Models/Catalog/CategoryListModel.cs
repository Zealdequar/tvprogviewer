using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a category list model
    /// </summary>
    public partial record CategoryListModel : BasePagedListModel<CategoryModel>
    {
    }
}