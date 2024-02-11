using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated tvchannel list model to add to the tvchannel
    /// </summary>
    public partial record AddAssociatedTvChannelListModel : BasePagedListModel<TvChannelModel>
    {
    }
}