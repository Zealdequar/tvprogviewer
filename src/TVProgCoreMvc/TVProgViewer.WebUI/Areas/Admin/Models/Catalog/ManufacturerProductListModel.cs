using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer product list model
    /// </summary>
    public partial record ManufacturerProductListModel : BasePagedListModel<ManufacturerProductModel>
    {
    }
}