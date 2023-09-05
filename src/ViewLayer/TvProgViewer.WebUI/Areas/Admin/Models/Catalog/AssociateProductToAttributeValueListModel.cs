using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product list model to associate to the product attribute value
    /// </summary>
    public partial record AssociateProductToAttributeValueListModel : BasePagedListModel<ProductModel>
    {
    }
}