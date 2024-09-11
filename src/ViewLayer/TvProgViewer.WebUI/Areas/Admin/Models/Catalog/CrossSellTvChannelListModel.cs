using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a cross-sell tvChannel list model
    /// </summary>
    public partial record CrossSellTvChannelListModel : BasePagedListModel<CrossSellTvChannelModel>
    {
    }
}