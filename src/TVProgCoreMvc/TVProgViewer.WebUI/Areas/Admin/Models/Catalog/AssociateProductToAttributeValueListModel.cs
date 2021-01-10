using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product list model to associate to the product attribute value
    /// </summary>
    public partial record AssociateProductToAttributeValueListModel : BasePagedListModel<ProductModel>
    {
    }
}