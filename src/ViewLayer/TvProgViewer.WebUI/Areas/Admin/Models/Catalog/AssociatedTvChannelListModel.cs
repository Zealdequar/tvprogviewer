using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated tvchannel list model
    /// </summary>
    public partial record AssociatedTvChannelListModel : BasePagedListModel<AssociatedTvChannelModel>
    {
    }
}