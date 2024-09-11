using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a list model of tvChannels that use the tvChannel attribute
    /// </summary>
    public partial record TvChannelAttributeTvChannelListModel : BasePagedListModel<TvChannelAttributeTvChannelModel>
    {
    }
}