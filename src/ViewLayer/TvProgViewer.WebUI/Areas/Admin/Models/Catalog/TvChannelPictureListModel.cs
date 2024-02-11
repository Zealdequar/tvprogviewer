using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel picture list model
    /// </summary>
    public partial record TvChannelPictureListModel : BasePagedListModel<TvChannelPictureModel>
    {
    }
}