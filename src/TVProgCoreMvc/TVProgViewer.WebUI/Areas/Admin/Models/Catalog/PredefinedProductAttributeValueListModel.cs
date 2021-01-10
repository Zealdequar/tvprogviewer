using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a predefined product attribute value list model
    /// </summary>
    public partial record PredefinedProductAttributeValueListModel : BasePagedListModel<PredefinedProductAttributeValueModel>
    {
    }
}