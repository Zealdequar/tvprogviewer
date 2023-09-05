using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a stock quantity history list model
    /// </summary>
    public partial record StockQuantityHistoryListModel : BasePagedListModel<StockQuantityHistoryModel>
    {
    }
}