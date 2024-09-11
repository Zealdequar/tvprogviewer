using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a low stock tvChannel list model
    /// </summary>
    public partial record LowStockTvChannelListModel : BasePagedListModel<LowStockTvChannelModel>
    {
    }
}