using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a required tvchannel list model to add to the tvchannel
    /// </summary>
    public partial record AddRequiredTvChannelListModel : BasePagedListModel<TvChannelModel>
    {
    }
}