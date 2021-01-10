using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute mapping list model
    /// </summary>
    public partial record ProductAttributeMappingListModel : BasePagedListModel<ProductAttributeMappingModel>
    {
    }
}