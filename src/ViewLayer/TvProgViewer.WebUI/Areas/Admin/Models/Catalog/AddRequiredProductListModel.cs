using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a required product list model to add to the product
    /// </summary>
    public partial record AddRequiredProductListModel : BasePagedListModel<ProductModel>
    {
    }
}