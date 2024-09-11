using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel specification attribute list model
    /// </summary>
    public partial record TvChannelSpecificationAttributeListModel : BasePagedListModel<TvChannelSpecificationAttributeModel>
    {
    }
}