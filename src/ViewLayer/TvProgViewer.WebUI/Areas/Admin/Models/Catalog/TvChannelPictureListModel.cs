using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel picture list model
    /// </summary>
    public partial record TvChannelPictureListModel : BasePagedListModel<TvChannelPictureModel>
    {
    }
}