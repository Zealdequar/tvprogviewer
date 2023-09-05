using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product review list model
    /// </summary>
    public partial record ProductReviewListModel : BasePagedListModel<ProductReviewModel>
    {
    }
}