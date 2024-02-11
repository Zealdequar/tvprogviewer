using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel specification attribute list model
    /// </summary>
    public partial record TvChannelSpecificationAttributeListModel : BasePagedListModel<TvChannelSpecificationAttributeModel>
    {
    }
}