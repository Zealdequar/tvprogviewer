using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a cross-sell tvChannel list model to add to the tvChannel
    /// </summary>
    public partial record AddCrossSellTvChannelListModel : BasePagedListModel<TvChannelModel>
    {
    }
}