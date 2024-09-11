using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute list model
    /// </summary>
    public partial record TvChannelAttributeListModel : BasePagedListModel<TvChannelAttributeModel>
    {
    }
}