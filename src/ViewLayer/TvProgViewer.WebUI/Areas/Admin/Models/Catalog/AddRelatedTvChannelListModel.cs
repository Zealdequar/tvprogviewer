using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a related tvchannel list model to add to the tvchannel
    /// </summary>
    public partial record AddRelatedTvChannelListModel : BasePagedListModel<TvChannelModel>
    {
    }
}