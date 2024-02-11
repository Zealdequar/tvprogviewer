using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel review and review type mapping search model
    /// </summary>
    public partial record TvChannelReviewReviewTypeMappingSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelReviewId { get; set; }

        public bool IsAnyReviewTypes { get; set; }

        #endregion
    }
}
