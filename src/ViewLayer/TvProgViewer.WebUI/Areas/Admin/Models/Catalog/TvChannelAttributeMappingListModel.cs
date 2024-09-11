using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute mapping list model
    /// </summary>
    public partial record TvChannelAttributeMappingListModel : BasePagedListModel<TvChannelAttributeMappingModel>
    {
    }
}