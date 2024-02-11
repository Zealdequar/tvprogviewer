using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel list model to associate to the tvchannel attribute value
    /// </summary>
    public partial record AssociateTvChannelToAttributeValueListModel : BasePagedListModel<TvChannelModel>
    {
    }
}