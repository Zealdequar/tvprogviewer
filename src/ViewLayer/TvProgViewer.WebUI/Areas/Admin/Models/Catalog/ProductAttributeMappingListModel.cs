using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute mapping list model
    /// </summary>
    public partial record ProductAttributeMappingListModel : BasePagedListModel<ProductAttributeMappingModel>
    {
    }
}