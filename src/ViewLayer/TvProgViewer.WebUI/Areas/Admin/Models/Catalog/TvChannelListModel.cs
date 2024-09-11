using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel list model
    /// </summary>
    public partial record TvChannelListModel : BasePagedListModel<TvChannelModel>
    {
    }
}