using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product list model
    /// </summary>
    public partial record ProductListModel : BasePagedListModel<ProductModel>
    {
    }
}