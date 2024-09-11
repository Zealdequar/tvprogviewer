using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute combination list model
    /// </summary>
    public partial record TvChannelAttributeCombinationListModel : BasePagedListModel<TvChannelAttributeCombinationModel>
    {
    }
}