using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute combination list model
    /// </summary>
    public partial record ProductAttributeCombinationListModel : BasePagedListModel<ProductAttributeCombinationModel>
    {
    }
}