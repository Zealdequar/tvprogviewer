using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute value list model
    /// </summary>
    public partial record ProductAttributeValueListModel : BasePagedListModel<ProductAttributeValueModel>
    {
    }
}