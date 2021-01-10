using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product list model
    /// </summary>
    public partial record ProductListModel : BasePagedListModel<ProductModel>
    {
    }
}