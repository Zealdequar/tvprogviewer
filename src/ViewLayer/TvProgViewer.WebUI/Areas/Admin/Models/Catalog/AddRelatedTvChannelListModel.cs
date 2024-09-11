using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a related tvChannel list model to add to the tvChannel
    /// </summary>
    public partial record AddRelatedTvChannelListModel : BasePagedListModel<TvChannelModel>
    {
    }
}