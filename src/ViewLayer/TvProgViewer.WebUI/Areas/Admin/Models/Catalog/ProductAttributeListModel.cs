using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute list model
    /// </summary>
    public partial record ProductAttributeListModel : BasePagedListModel<ProductAttributeModel>
    {
    }
}