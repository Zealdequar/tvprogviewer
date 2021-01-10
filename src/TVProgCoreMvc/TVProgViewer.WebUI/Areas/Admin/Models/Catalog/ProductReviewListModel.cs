using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product review list model
    /// </summary>
    public partial record ProductReviewListModel : BasePagedListModel<ProductReviewModel>
    {
    }
}