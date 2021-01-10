using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product review and review type mapping search model
    /// </summary>
    public partial record ProductReviewReviewTypeMappingSearchModel : BaseSearchModel
    {
        #region Properties

        public int ProductReviewId { get; set; }

        public bool IsAnyReviewTypes { get; set; }

        #endregion
    }
}
