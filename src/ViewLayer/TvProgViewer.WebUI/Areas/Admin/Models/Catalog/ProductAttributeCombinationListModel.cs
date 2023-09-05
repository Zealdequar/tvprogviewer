using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute combination list model
    /// </summary>
    public partial record ProductAttributeCombinationListModel : BasePagedListModel<ProductAttributeCombinationModel>
    {
    }
}