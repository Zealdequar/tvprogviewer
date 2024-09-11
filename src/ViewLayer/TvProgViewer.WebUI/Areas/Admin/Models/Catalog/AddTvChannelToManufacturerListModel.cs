using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel list model to add to the manufacturer
    /// </summary>
    public partial record AddTvChannelToManufacturerListModel : BasePagedListModel<TvChannelModel>
    {
    }
}