using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a warehouse list model
    /// </summary>
    public partial record WarehouseListModel : BasePagedListModel<WarehouseModel>
    {
    }
}