using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a list model of products that use the product attribute
    /// </summary>
    public partial record ProductAttributeProductListModel : BasePagedListModel<ProductAttributeProductModel>
    {
    }
}