using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel list model to associate to the tvChannel attribute value
    /// </summary>
    public partial record AssociateTvChannelToAttributeValueListModel : BasePagedListModel<TvChannelModel>
    {
    }
}