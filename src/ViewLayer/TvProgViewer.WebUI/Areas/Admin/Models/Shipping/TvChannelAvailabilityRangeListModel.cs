using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a tvChannel availability range list model
    /// </summary>
    public partial record TvChannelAvailabilityRangeListModel : BasePagedListModel<TvChannelAvailabilityRangeModel>
    {
    }
}