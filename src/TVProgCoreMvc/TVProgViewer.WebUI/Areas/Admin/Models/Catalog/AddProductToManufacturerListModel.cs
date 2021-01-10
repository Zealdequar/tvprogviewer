using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product list model to add to the manufacturer
    /// </summary>
    public partial record AddProductToManufacturerListModel : BasePagedListModel<ProductModel>
    {
    }
}