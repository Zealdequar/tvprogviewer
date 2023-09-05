using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a low stock product list model
    /// </summary>
    public partial record LowStockProductListModel : BasePagedListModel<LowStockProductModel>
    {
    }
}