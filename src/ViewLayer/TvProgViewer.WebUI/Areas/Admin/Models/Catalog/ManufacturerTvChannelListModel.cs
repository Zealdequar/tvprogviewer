using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer tvchannel list model
    /// </summary>
    public partial record ManufacturerTvChannelListModel : BasePagedListModel<ManufacturerTvChannelModel>
    {
    }
}