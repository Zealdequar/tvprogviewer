using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product review and review type mapping list model
    /// </summary>
    public partial record ProductReviewReviewTypeMappingListModel : BasePagedListModel<ProductReviewReviewTypeMappingModel>
    {
    }
}
