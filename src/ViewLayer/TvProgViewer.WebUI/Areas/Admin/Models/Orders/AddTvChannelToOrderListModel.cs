using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a tvChannel list model to add to the order
    /// </summary>
    public partial record AddTvChannelToOrderListModel : BasePagedListModel<TvChannelModel>
    {
    }
}