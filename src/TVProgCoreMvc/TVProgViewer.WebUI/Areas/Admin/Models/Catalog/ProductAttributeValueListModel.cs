using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute value list model
    /// </summary>
    public partial record ProductAttributeValueListModel : BasePagedListModel<ProductAttributeValueModel>
    {
    }
}