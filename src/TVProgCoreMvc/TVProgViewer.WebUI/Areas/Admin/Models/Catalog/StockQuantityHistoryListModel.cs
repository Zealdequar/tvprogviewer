using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a stock quantity history list model
    /// </summary>
    public partial record StockQuantityHistoryListModel : BasePagedListModel<StockQuantityHistoryModel>
    {
    }
}