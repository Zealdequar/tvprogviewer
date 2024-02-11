using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a related tvchannel list model
    /// </summary>
    public partial record RelatedTvChannelListModel : BasePagedListModel<RelatedTvChannelModel>
    {
    }
}