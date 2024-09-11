using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated tvChannel list model to add to the tvChannel
    /// </summary>
    public partial record AddAssociatedTvChannelListModel : BasePagedListModel<TvChannelModel>
    {
    }
}