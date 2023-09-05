using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product picture list model
    /// </summary>
    public partial record ProductPictureListModel : BasePagedListModel<ProductPictureModel>
    {
    }
}