using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a warehouse list model
    /// </summary>
    public partial record WarehouseListModel : BasePagedListModel<WarehouseModel>
    {
    }
}