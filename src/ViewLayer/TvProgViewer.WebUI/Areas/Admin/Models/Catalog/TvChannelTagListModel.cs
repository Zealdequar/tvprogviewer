using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel tag list model
    /// </summary>
    public partial record TvChannelTagListModel : BasePagedListModel<TvChannelTagModel>
    {
    }
}