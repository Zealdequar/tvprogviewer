using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product specification attribute list model
    /// </summary>
    public partial record ProductSpecificationAttributeListModel : BasePagedListModel<ProductSpecificationAttributeModel>
    {
    }
}