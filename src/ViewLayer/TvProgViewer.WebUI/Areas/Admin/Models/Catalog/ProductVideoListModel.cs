using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product video list model
    /// </summary>
    public partial record ProductVideoListModel : BasePagedListModel<ProductVideoModel>
    {
    }
}