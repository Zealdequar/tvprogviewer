using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated product list model to add to the product
    /// </summary>
    public partial record AddAssociatedProductListModel : BasePagedListModel<ProductModel>
    {
    }
}