using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel list model to add to the manufacturer
    /// </summary>
    public partial record AddTvChannelToManufacturerListModel : BasePagedListModel<TvChannelModel>
    {
    }
}