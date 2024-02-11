using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel list model to add to the category
    /// </summary>
    public partial record AddTvChannelToCategoryListModel : BasePagedListModel<TvChannelModel>
    {
    }
}