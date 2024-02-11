using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a category tvchannel list model
    /// </summary>
    public partial record CategoryTvChannelListModel : BasePagedListModel<CategoryTvChannelModel>
    {
    }
}