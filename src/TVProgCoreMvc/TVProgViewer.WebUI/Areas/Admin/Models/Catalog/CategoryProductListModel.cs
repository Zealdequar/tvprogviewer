using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a category product list model
    /// </summary>
    public partial record CategoryProductListModel : BasePagedListModel<CategoryProductModel>
    {
    }
}