using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated product list model to add to the product
    /// </summary>
    public partial record AddAssociatedProductListModel : BasePagedListModel<ProductModel>
    {
    }
}