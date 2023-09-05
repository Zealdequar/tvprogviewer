using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a predefined product attribute value list model
    /// </summary>
    public partial record PredefinedProductAttributeValueListModel : BasePagedListModel<PredefinedProductAttributeValueModel>
    {
    }
}