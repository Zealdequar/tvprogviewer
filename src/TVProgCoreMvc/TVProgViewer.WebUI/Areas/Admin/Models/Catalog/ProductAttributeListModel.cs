using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute list model
    /// </summary>
    public partial record ProductAttributeListModel : BasePagedListModel<ProductAttributeModel>
    {
    }
}